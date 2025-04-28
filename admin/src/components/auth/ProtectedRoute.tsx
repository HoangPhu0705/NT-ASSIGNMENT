import React, { useEffect, useState } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "oidc-react";

const ProtectedRoute: React.FC = () => {
  const auth = useAuth();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);

  useEffect(() => {
    const checkAuth = async () => {
      if (!auth.isLoading && auth.userData) {
        const roles = auth.userData.profile.role;
        const isAdmin = Array.isArray(roles)
          ? roles.includes("Admin")
          : roles === "Admin";
        setIsAuthenticated(isAdmin);
      } else {
        setIsAuthenticated(false);
      }
    };

    checkAuth();
  }, [auth.isLoading, auth.userData]);

  if (isAuthenticated === null) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        Loading...
      </div>
    );
  }

  return isAuthenticated ? <Outlet /> : <Navigate to="/" replace />;
};

export default ProtectedRoute;
