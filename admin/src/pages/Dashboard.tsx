import React from "react";

const Dashboard: React.FC = () => {
  const handleLogout = () => {
    const params = new URLSearchParams({
      id_token_hint: localStorage.getItem("id_token") || "",
      post_logout_redirect_uri: "http://localhost:5173/logout-callback",
    });

    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("id_token");

    window.location.href = `https://localhost:7130/connect/logout?${params.toString()}`;
  };

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
      <p className="mt-4 text-gray-600">
        Welcome, Admin! Manage your application here.
      </p>
      <button
        onClick={handleLogout}
        className="mt-6 rounded-md bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-500"
      >
        Log Out
      </button>
    </div>  
  );
};

export default Dashboard;
