import React from "react";
import {
  createBrowserRouter,
  RouterProvider as ReactRouterProvider,
} from "react-router-dom";
import { AuthProvider, useAuth } from "oidc-react";
import ErrorPage from "../components/common/ErrorPage";
import Layout from "../components/layout/Layout";
import ProductPage from "../pages/ProductPage";
import Dashboard from "../pages/Dashboard";
import Category from "../pages/Category";
import Hero from "../pages/Hero";
import Customer from "../pages/Customer";
import ProtectedRoute from "@/components/auth/ProtectedRoute";
import { Spinner } from "@/components/ui";

// Component to handle sign-in callback
const SignInCallback = () => {
  const auth = useAuth();

  React.useEffect(() => {
    const processSignIn = async () => {
      try {
        console.log("Processing callback with URL:", window.location.href);
        await auth.userManager.signinRedirectCallback();
        console.log("Callback processed, redirecting to /dashboard");
        window.history.replaceState(null, "", "/dashboard");
        window.location.href = "/dashboard";
      } catch (error) {
        console.error("Sign-in callback error:", error);
      }
    };

    processSignIn();
  }, [auth]);

  return (
    <div className="w-full h-full flex justify-center items-center">
      <Spinner />
    </div>
  );
};

// Component to handle sign-out callback
const SignOutCallback = () => {
  const auth = useAuth();

  React.useEffect(() => {
    const processSignOut = async () => {
      try {
        await auth.userManager.signoutRedirectCallback();
        auth.userManager.clearStaleState();
        auth.userManager.removeUser();
        window.history.replaceState(null, "", "/");
        window.location.href = "/";
      } catch (error) {
        console.error("Sign-out callback error:", error);
      }
    };

    processSignOut();
  }, [auth]);

  return (
    <div className="w-full h-full flex justify-center items-center">
      <Spinner />
    </div>
  );
};

const oidcConfig = {
  authority: "https://localhost:7130",
  clientId: "admin-web-client",
  redirectUri: "https://localhost:5173/signin-oidc",
  postLogoutRedirectUri: "https://localhost:5173/signout-callback-oidc",
  responseType: "code",
  scope: "openid email profile roles offline_access api",
  onSignIn: async (user) => {
    console.log("Sign-in successful, user:", user);
    window.history.replaceState(null, "", "/dashboard");
  },
  autoSignIn: false,
};

const router = createBrowserRouter([
  {
    path: "/",
    element: <Hero />,
    errorElement: <ErrorPage />,
  },
  {
    path: "/signin-oidc",
    element: <SignInCallback />,
  },
  {
    path: "/signout-callback-oidc",
    element: <SignOutCallback />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        path: "/dashboard",
        element: <Layout children={<Dashboard />} />,
        errorElement: <ErrorPage />,
      },
      {
        path: "/product",
        element: <Layout children={<ProductPage />} />,
        errorElement: <ErrorPage />,
      },
      {
        path: "/category",
        element: <Layout children={<Category />} />,
        errorElement: <ErrorPage />,
      },
      {
        path: "/customers",
        element: <Layout children={<Customer />} />,
        errorElement: <ErrorPage />,
      },
      {
        path: "/settings",
        element: <Layout children={<ProductPage />} />,
        errorElement: <ErrorPage />,
      },
    ],
  },
]);

const RouterProvider: React.FC = () => {
  return (
    <AuthProvider {...oidcConfig}>
      <ReactRouterProvider router={router} />
    </AuthProvider>
  );
};

export default RouterProvider;
