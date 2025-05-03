import React, { useEffect, useState } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "oidc-react";

const ProtectedRoute: React.FC = () => {
  const auth = useAuth();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);

  useEffect(() => {
    const checkAuth = async () => {
      console.info("User data:", auth.userData);
      if (!auth.isLoading && auth.userData) {
        console.info("User profile:", auth.userData.profile);
        const roles = auth.userData.profile.role;
        const isAdmin = Array.isArray(roles)
          ? roles.includes("Admin")
          : roles === "Admin";
        setIsAuthenticated(isAdmin);
      } else if (!auth.isLoading) {
        console.info("No user data or still loading");
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

  console.info("authen final" + isAuthenticated);
  return isAuthenticated ? <Outlet /> : <Navigate to="/" replace />;
};

export default ProtectedRoute;
