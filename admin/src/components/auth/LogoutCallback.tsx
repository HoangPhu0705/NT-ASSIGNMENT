import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const LogoutCallback: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("id_token");
    navigate("/");
  }, [navigate]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      Logging out...
    </div>
  );
};

export default LogoutCallback;
