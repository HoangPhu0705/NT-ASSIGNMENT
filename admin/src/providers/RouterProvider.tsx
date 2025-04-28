import React from "react";
import {
  createBrowserRouter,
  RouterProvider as ReactRouterProvider,
} from "react-router-dom";
import { AuthProvider } from "oidc-react";
import ErrorPage from "../components/common/ErrorPage";
import Layout from "../components/layout/Layout";
import ProductPage from "../pages/ProductPage";
import Dashboard from "../pages/Dashboard";
import Category from "../pages/Category";
import User from "../pages/User";
import Hero from "../pages/Hero";
import LogoutCallback from "../components/auth/LogoutCallback";
import ProtectedRoute from "../components/auth/ProtectedRoute";

const oidcConfig = {
  authority: "https://localhost:7130",
  clientId: "admin-web-client",
  redirectUri: "http://localhost:5173/signin-oidc",
  postLogoutRedirectUri: "http://localhost:5173/signout-callback-oidc",
  responseType: "code",
  scope: "openid email profile roles offline_access api",
  onSignIn: () => {
    window.history.replaceState({}, document.title, window.location.pathname);
    window.location.href = "/dashboard";
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
    path: "/signout-callback-oidc",
    element: <LogoutCallback />,
    errorElement: <ErrorPage />,
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
        path: "/users",
        element: <Layout children={<User />} />,
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
