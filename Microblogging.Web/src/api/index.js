
const API_URL = "/api";

export async function fetchTweets(userId) {
  const response = await fetch(`${API_URL}/timeline`, {
    headers: {
      "X-User-Id": userId,
    },
  });
  return await response.json();
}

export async function postTweet(userId, content) {
  const response = await fetch(`${API_URL}/tweets`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-User-Id": userId,
    },
    body: JSON.stringify({ Content : content }),
  });
  return await response.json();
}

export async function fetchFollowableUsers(userId) {
  const response = await fetch(`${API_URL}/followable_users`, {
    headers: {
      "X-User-Id": userId,
    },
  });

  if (!response.ok) {
    throw new Error("Error fetching followable users");
  }

  return await response.json(); // Espera un array de UserId strings
}

export async function followUser(userId, followeeId) {
  const response = await fetch(`${API_URL}/follow`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-User-Id": userId,
    },
    body: JSON.stringify({ FollowedUserId : followeeId }),
  });

  if (!response.ok) {
    throw new Error("Error following user");
  }
}
