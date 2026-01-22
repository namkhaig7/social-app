"use client";

import { useQuery, useMutation } from "convex/react";
import { useState } from "react";
// IF you copied the convex folder, import from "@/convex/_generated/api"
// IF NOT, we will use 'any' below to prevent crashes for now.
import { api } from "@/convex/_generated/api";
// OR if you didn't copy the folder yet, comment the line above and uncomment this:
// const api: any = {};

import { Check, X, Loader2 } from "lucide-react";

export default function AdminDashboard() {
  // 1. Fetch Pending Events
  // Note: If you didn't copy the _generated folder, you might see red lines here.
  // It will still work in the browser if the string names match your backend.
  const pendingEvents = useQuery(api.events.getPendingEvents);

  // 2. Setup Mutation
  const updateStatus = useMutation(api.events.updateStatus);
  const [processingId, setProcessingId] = useState<string | null>(null);

  const handleDecision = async (id: any, status: "APPROVED" | "REJECTED") => {
    setProcessingId(id);
    try {
      await updateStatus({
        id,
        status,
        adminComment:
          status === "APPROVED" ? "Great event!" : "Please add more details.",
      });
    } catch (error) {
      alert("Failed to update status");
      console.error(error);
    } finally {
      setProcessingId(null);
    }
  };

  if (pendingEvents === undefined) {
    return (
      <div className="flex h-screen items-center justify-center">
        Loading...
      </div>
    );
  }

  return (
    <main className="p-8 max-w-6xl mx-auto">
      <div className="mb-8 flex justify-between items-center">
        <h1 className="text-3xl font-bold">Admin Dashboard</h1>
        <span className="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm font-medium">
          {pendingEvents.length} Pending Requests
        </span>
      </div>

      <div className="grid gap-6">
        {pendingEvents.length === 0 ? (
          <p className="text-gray-500 italic">No pending events to review.</p>
        ) : (
          pendingEvents.map((event: any) => (
            <div
              key={event._id}
              className="bg-white border border-gray-200 rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow"
            >
              <div className="flex justify-between items-start">
                <div>
                  <h2 className="text-xl font-semibold text-gray-900">
                    {event.title}
                  </h2>
                  <p className="text-sm text-gray-500 mb-2">
                    Requested by:{" "}
                    <span className="font-medium text-gray-700">
                      {event.owner?.fnamem} {event.owner?.lnamem}
                    </span>{" "}
                    • {event.date}
                  </p>
                  <p className="text-gray-600 mt-2">{event.description}</p>

                  <div className="mt-4 flex gap-2 text-sm text-gray-500">
                    <span className="bg-gray-100 px-2 py-1 rounded">
                      📍 {event.location}
                    </span>
                    <span className="bg-gray-100 px-2 py-1 rounded">
                      🏷️ {event.category}
                    </span>
                  </div>
                </div>

                {/* Action Buttons */}
                <div className="flex flex-col gap-2 ml-4">
                  <button
                    onClick={() => handleDecision(event._id, "APPROVED")}
                    disabled={!!processingId}
                    className="flex items-center gap-2 bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md transition-colors disabled:opacity-50"
                  >
                    {processingId === event._id ? (
                      <Loader2 className="animate-spin w-4 h-4" />
                    ) : (
                      <Check className="w-4 h-4" />
                    )}
                    Approve
                  </button>

                  <button
                    onClick={() => handleDecision(event._id, "REJECTED")}
                    disabled={!!processingId}
                    className="flex items-center gap-2 bg-red-50 hover:bg-red-100 text-red-600 border border-red-200 px-4 py-2 rounded-md transition-colors disabled:opacity-50"
                  >
                    <X className="w-4 h-4" />
                    Reject
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </main>
  );
}
