import { useState } from "react";
import { postTweet } from "../api";

export default function TweetForm({ userId, onTweeted, showNotification }) {
  const [content, setContent] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await postTweet(userId, content);
      setContent("");
      showNotification("Tweet publicado con éxito.");
      onTweeted(); // Refresca el timeline
    } catch (error) {
      showNotification("Error al publicar tweet.");
      console.error(error);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-2">
      <textarea
        value={content}
        onChange={(e) => setContent(e.target.value)}
        className="border p-2 rounded"
        placeholder="¿Qué estás pensando?"
      />
      <button
        type="submit"
        className="bg-blue-600 text-white px-4 py-2 rounded"
      >
        Twittear
      </button>
    </form>
  );
}
