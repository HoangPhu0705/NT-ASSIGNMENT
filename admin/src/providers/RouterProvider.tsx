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
import Hero from "../pages/Hero";
import LogoutCallback from "../components/auth/LogoutCallback";
import Customer from "../pages/Customer";

const oidcConfig = {
  authority: "https://localhost:7130",
  clientId: "admin-web-client",
  clientSecret: "admin-secret",
  redirectUri: "https://localhost:5173/dashboard",
  postLogoutRedirectUri: "https://localhost:5173/signout-callback-oidc",
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
  // {
  //   element: <ProtectedRoute />,
  //   children: [
  //     // {
  //     //   path: "/dashboard",
  //     //   element: <Layout children={<Dashboard />} />,
  //     //   errorElement: <ErrorPage />,
  //     // },
  //     // {
  //     //   path: "/product",
  //     //   element: <Layout children={<ProductPage />} />,
  //     //   errorElement: <ErrorPage />,
  //     // },
  //     // {
  //     //   path: "/category",
  //     //   element: <Layout children={<Category />} />,
  //     //   errorElement: <ErrorPage />,
  //     // },
  //     // {
  //     //   path: "/users",
  //     //   element: <Layout children={<User />} />,
  //     //   errorElement: <ErrorPage />,
  //     // },
  //     // {
  //     //   path: "/settings",
  //     //   element: <Layout children={<ProductPage />} />,
  //     //   errorElement: <ErrorPage />,
  //     // },
  //   ],
  // },
]);

const RouterProvider: React.FC = () => {
  return (
    <AuthProvider {...oidcConfig}>
      <ReactRouterProvider router={router} />
    </AuthProvider>
  );
};

export default RouterProvider;
