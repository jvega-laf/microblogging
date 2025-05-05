// src/App.jsx
import React, { useState } from "react";
import TweetForm from "./components/TweetForm";
import Timeline from "./components/Timeline";
import UserList from "./components/UserList";

export default function App() {
  const [userId] = useState(() => {
    let id = localStorage.getItem("userId");
    if (!id) {
      id = crypto.randomUUID();
      localStorage.setItem("userId", id);
    }
    return id;
  });
  const [activeTab, setActiveTab] = useState("tweet");
  const [refreshTimeline, setRefreshTimeline] = useState(false);
  const [refreshUsers, setRefreshUsers] = useState(false);
  const [notification, setNotification] = useState(null);

  const handleTweetPosted = async (success) => {
    if (success) {
      setNotification({
        type: "success",
        message: "Tweet publicado correctamente.",
      });
      setRefreshTimeline((r) => !r);
    } else {
      setNotification({
        type: "error",
        message: "Error al publicar el tweet.",
      });
    }
    setTimeout(() => setNotification(null), 3000);
  };

  const showNotification = (msg) => {
    setNotification(msg);
    setTimeout(() => setNotification(""), 2000); // Oculta luego de 3s
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white shadow-lg rounded-lg w-full max-w-xl p-6">
        <h1 className="text-2xl font-bold mb-4 text-center">
          Microblogging App
        </h1>
        <p className="text-center text-sm text-gray-500 mb-4">
          Tu ID: <code>{userId}</code>
        </p>

        {notification && (
          <div
            className={`mb-4 p-2 rounded text-center ${
              notification.type === "success"
                ? "bg-green-100 text-green-800"
                : "bg-red-100 text-red-800"
            }`}
          >
            {notification.message}
          </div>
        )}

        {/* Tabs */}
        <div className="flex justify-center mb-6">
          <button
            onClick={() => setActiveTab("tweet")}
            className={`px-4 py-2 ${
              activeTab === "tweet"
                ? "border-b-2 border-blue-500 text-blue-500"
                : "text-gray-600"
            }`}
          >
            Twittear
          </button>
          <button
            onClick={() => setActiveTab("timeline")}
            className={`px-4 py-2 ${
              activeTab === "timeline"
                ? "border-b-2 border-blue-500 text-blue-500"
                : "text-gray-600"
            }`}
          >
            Timeline
          </button>
          <button
            onClick={() => setActiveTab("users")}
            className={`px-4 py-2 ${
              activeTab === "users"
                ? "border-b-2 border-blue-500 text-blue-500"
                : "text-gray-600"
            }`}
          >
            Usuarios
          </button>
        </div>

        {/* Content */}
        {activeTab === "tweet" && (
          <TweetForm
            userId={userId}
            onTweeted={handleTweetPosted}
            showNotification={showNotification}
          />
        )}

        {activeTab === "timeline" && (
          <div>
            <button
              onClick={() => setRefreshTimeline((r) => !r)}
              className="mb-4 px-3 py-1 bg-blue-500 text-white rounded"
            >
              Recargar Timeline
            </button>
            <Timeline refresh={refreshTimeline} userId={userId} />
          </div>
        )}

        {activeTab === "users" && (
          <div>
            <button
              onClick={() => setRefreshUsers((r) => !r)}
              className="mb-4 px-3 py-1 bg-blue-500 text-white rounded"
            >
              Recargar Usuarios
            </button>
            <UserList
              refresh={refreshUsers}
              userId={userId}
              showNotification={showNotification}
            />
          </div>
        )}
      </div>
    </div>
  );
}
