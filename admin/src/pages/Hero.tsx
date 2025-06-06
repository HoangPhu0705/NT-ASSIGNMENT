import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "oidc-react";

function Hero() {
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      if (!auth.isLoading && auth.userData) {
        const roles = auth.userData.profile.role;
        const isAdmin = Array.isArray(roles)
          ? roles.includes("Admin")
          : roles === "Admin";
        if (isAdmin) {
          console.info("Navigating to /dashboard for authenticated admin");
          // Replace history to prevent back navigation to /
          window.history.replaceState(null, "", "/dashboard");
          navigate("/dashboard", { replace: true });
        }
      }
    };

    checkAuth();
  }, [auth.isLoading, auth.userData, navigate]);

  const handleLogin = () => {
    auth.signIn();
  };

  return (
    <div className="bg-gray-50 min-h-screen flex items-center justify-center">
      <div className="relative isolate px-6 pt-14 lg:px-8">
        <div
          className="absolute inset-x-0 -top-40 -z-10 transform-gpu overflow-hidden blur-3xl sm:-top-80"
          aria-hidden="true"
        >
          <div className="relative left-[calc(50%-11rem)] aspect-[1155/678] w-[36.125rem] -translate-x-1/2 rotate-[30deg] bg-gradient-to-tr from-[#93c5fd] to-[#3b82f6] opacity-30 sm:left-[calc(50%-30rem)] sm:w-[72.1875rem]"></div>
        </div>

        {/* Content */}
        <div className="mx-auto max-w-2xl py-32 sm:py-48 lg:py-56 text-center">
          <h1 className="text-balance text-4xl font-bold tracking-tight text-gray-900 sm:text-6xl">
            Welcome back Nextech Admin
          </h1>
          <p className="mt-6 text-lg leading-8 text-gray-600">
            Manage your application with ease. Log in to access powerful tools
          </p>
          <div className="mt-10 flex items-center justify-center">
            <button
              onClick={handleLogin}
              className="rounded-md bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus-visible:outline focus-visible:outline-offset-2 focus-visible:outline-blue-600 hover:cursor-pointer"
            >
              Log In
            </button>
          </div>
        </div>

        {/* Bottom gradient */}
        <div
          className="absolute inset-x-0 top-[calc(100%-13rem)] -z-10 transform-gpu overflow-hidden blur-3xl sm:top-[calc(100%-30rem)]"
          aria-hidden="true"
        >
          <div className="relative left-[calc(50%+3rem)] aspect-[1155/678] w-[36.125rem] -translate-x-1/2 bg-gradient-to-tr from-[#93c5fd] to-[#3b82f6] opacity-30 sm:left-[calc(50%+36rem)] sm:w-[72.1875rem]"></div>
        </div>
      </div>
    </div>
  );
}

export default Hero;
