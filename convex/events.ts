import { query, mutation } from "./_generated/server";
import { v } from "convex/values";

// 1. FOR STUDENTS: Get Approved Events (with search & filter)
export const getApprovedEvents = query({
  args: {
    search: v.optional(v.string()),
    category: v.optional(v.string()),
  },
  handler: async (ctx, args) => {
    // Start with all APPROVED events
    let events = await ctx.db
      .query("events")
      .withIndex("by_status", (q) => q.eq("status", "APPROVED"))
      .order("asc") // Order by creation time (default) or add .order() if index allows
      .collect();

    // Convex filtering happens in memory for complex text search usually,
    // or you use .filter() if scanning.
    // Since we already fetched them, let's filter in JS (fine for small/medium apps)
    if (args.category) {
      events = events.filter((e) => e.category === args.category);
    }

    if (args.search) {
      const searchLower = args.search.toLowerCase();
      events = events.filter((e) =>
        e.title.toLowerCase().includes(searchLower)
      );
    }

    // "Join" the owner data manually
    // In Convex, we usually do this asynchronously
    const eventsWithUser = await Promise.all(
      events.map(async (event) => {
        // Assuming ownerId matches the public_hash in users table
        const owner = await ctx.db
          .query("users")
          .withIndex("by_hash", (q) => q.eq("public_hash", event.ownerId))
          .first();

        return {
          ...event,
          ownerName: owner ? `${owner.lnamem} ${owner.fnamem}` : "Unknown",
          // Generate a URL for the image if it exists
          imageUrl: event.imageStorageId
            ? await ctx.storage.getUrl(event.imageStorageId)
            : null,
        };
      })
    );

    return eventsWithUser;
  },
});

// 2. FOR ADMINS: Get Pending Requests
export const getPendingEvents = query({
  handler: async (ctx) => {
    const events = await ctx.db
      .query("events")
      .withIndex("by_status", (q) => q.eq("status", "PENDING"))
      .order("desc")
      .collect();

    // Fetch owner details for each
    return await Promise.all(
      events.map(async (e) => {
        const owner = await ctx.db
          .query("users")
          .withIndex("by_hash", (q) => q.eq("public_hash", e.ownerId))
          .first();
        return { ...e, owner };
      })
    );
  },
});

// Step 2: Save the event with the Storage ID
export const generateUploadUrl = mutation(async (ctx) => {
  return await ctx.storage.generateUploadUrl();
});

// 3. FOR SCHOOL STAFF: Create a new request
// Note: File upload logic is handled on frontend (generateUploadUrl -> POST -> save storageId)

export const createEvent = mutation({
  // 1. All fields from your form must be defined here
  args: {
    title: v.string(),
    description: v.string(),
    location: v.string(),
    category: v.string(),
    date: v.string(),
    ownerId: v.string(),
    time: v.optional(v.string()),
    imageStorageId: v.optional(v.id("_storage")),
  },
  handler: async (ctx, args) => {
    // 2. Now you can safely use args.description, args.location, etc.
    const newEventId = await ctx.db.insert("events", {
      title: args.title,
      description: args.description,
      location: args.location,
      category: args.category,
      date: args.date,
      time: args.time,
      ownerId: args.ownerId,
      imageStorageId: args.imageStorageId,
      status: "PENDING", // Default status for new requests
    });

    return newEventId;
  },
});

// 4. FOR ADMINS: Approve/Reject + Notify
export const updateStatus = mutation({
  args: {
    id: v.id("events"), // Convex automatically validates it's an Event ID
    status: v.union(v.literal("APPROVED"), v.literal("REJECTED")),
    adminComment: v.optional(v.string()),
  },
  handler: async (ctx, args) => {
    const { id, status, adminComment } = args;

    await ctx.db.patch(id, { status });

    const event = await ctx.db.get(id);
    if (!event) throw new Error("Event not found");

    // Create Notification
    await ctx.db.insert("notifications", {
      userId: event.ownerId,
      message: `Event "${event.title}" has been ${status.toLowerCase()}. ${adminComment || ""}`,
      isRead: false,
    });
  },
});

// 5. FOR SCHOOL: Stats
export const getSchoolStats = query({
  args: { ownerId: v.string() },
  handler: async (ctx, args) => {
    const events = await ctx.db
      .query("events")
      .withIndex("by_owner", (q) => q.eq("ownerId", args.ownerId))
      .collect();

    // Convex doesn't have a direct "groupBy" in the DB layer yet
    // But since one school won't have millions of events, we reduce in JS:
    const stats = events.reduce(
      (acc, curr) => {
        acc[curr.status] = (acc[curr.status] || 0) + 1;
        return acc;
      },
      {} as Record<string, number>
    );

    return stats;
  },
});

// 6. DELETE
export const deleteEvent = mutation({
  args: { id: v.id("events") },
  handler: async (ctx, args) => {
    // Optional: Delete the image from storage too if you want to save space
    const event = await ctx.db.get(args.id);
    if (event?.imageStorageId) {
      await ctx.storage.delete(event.imageStorageId);
    }
    await ctx.db.delete(args.id);
  },
});

// 8. FOR SCHOOL: Get My Events
export const getMyEvents = query({
  args: { ownerId: v.string() },
  handler: async (ctx, args) => {
    const events = await ctx.db
      .query("events")
      .withIndex("by_owner", (q) => q.eq("ownerId", args.ownerId))
      .order("desc")
      .collect();

    const school = await ctx.db
      .query("users")
      .withIndex("by_hash", (q) => q.eq("public_hash", args.ownerId))
      .first();

    return await Promise.all(
      events.map(async (e) => ({
        ...e,
        ownerName: school ? `${school.lnamem} ${school.fnamem}` : "Unknown",
        imageUrl: e.imageStorageId
          ? await ctx.storage.getUrl(e.imageStorageId)
          : null,
      }))
    );
  },
});
