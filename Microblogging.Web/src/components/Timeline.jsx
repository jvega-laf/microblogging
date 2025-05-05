import { useEffect, useState } from "react";
import { fetchTweets } from "../api";

export default function Timeline({ userId, refresh }) {
  const [tweets, setTweets] = useState([]);

  useEffect(() => {
    fetchTweets(userId)
      .then(setTweets)
      .catch((err) => console.error(err));
  }, [userId, refresh]);

  return (
    <div className="flex flex-col gap-4">
      {tweets.length === 0 && <p>No hay tweets aún.</p>}
      {tweets.map((tweet, idx) => (
        <div key={idx} className="border p-2 rounded bg-white shadow">
          <p className="font-bold">@{tweet.authorId}</p>
          <p>{tweet.content}</p>
        </div>
      ))}
    </div>
  );
}
