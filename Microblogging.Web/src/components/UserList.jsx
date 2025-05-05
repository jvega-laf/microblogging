import { useEffect, useState } from "react";
import { fetchFollowableUsers, followUser } from "../api";

export default function UserList({ userId, refresh, showNotification }) {
  const [users, setUsers] = useState([]);

  useEffect(() => {
    fetchFollowableUsers(userId)
      .then(setUsers)
      .catch((err) => {
        console.error(err);
        showNotification("Error al cargar usuarios.");
      });
  }, [userId, refresh, showNotification]);

  const handleFollow = async (followeeId) => {
    try {
      await followUser(userId, followeeId);
      showNotification(`Ahora sigues a ${followeeId}`);

      // 🔥 Actualiza la lista de usuarios localmente
      setUsers((prev) => prev.filter((uid) => uid !== followeeId));
    } catch (error) {
      showNotification("Error al seguir usuario.");
      console.error(error);
    }
  };
  return (
    <div className="flex flex-col gap-2">
      {users.length === 0 && <p>No hay usuarios disponibles para seguir.</p>}
      {users.map((uid) => (
        <div
          key={uid}
          className="flex justify-between items-center border p-2 rounded bg-white shadow"
        >
          <span>@{uid}</span>
          <button
            className="bg-green-600 text-white px-2 py-1 rounded"
            onClick={() => handleFollow(uid)}
          >
            Seguir
          </button>
        </div>
      ))}
    </div>
  );
}
