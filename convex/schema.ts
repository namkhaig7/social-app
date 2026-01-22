import { defineSchema, defineTable } from "convex/server";
import { v } from "convex/values";

export default defineSchema({
  events: defineTable({
    title: v.string(),
    description: v.string(),
    date: v.string(), // Keeping as string for simplicity, or v.number() for timestamps
    time: v.optional(v.string()),
    location: v.string(),
    category: v.string(),
    status: v.union(
      v.literal("PENDING"),
      v.literal("APPROVED"),
      v.literal("REJECTED")
    ),
    ownerId: v.string(), // This links to your User public_hash or ID
    imageStorageId: v.optional(v.id("_storage")),
  })
    .index("by_status", ["status"])
    .index("by_owner", ["ownerId"]),

  notifications: defineTable({
    userId: v.string(),
    message: v.string(),
    isRead: v.boolean(),
  }).index("by_user", ["userId"]),

  // Assuming you have a users table...
  users: defineTable({
    public_hash: v.string(),
    fnamem: v.string(),
    lnamem: v.string(),
    role: v.string(),
    email: v.string(),
  }).index("by_hash", ["public_hash"]),
});
