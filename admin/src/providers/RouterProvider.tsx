import ErrorPage from "@/components/common/ErrorPage";
import Layout from "@/components/layout/Layout";
import ProductPage from "@/pages/ProductPage";
import Dashboard from "@/pages/Dashboard";
import {
  createBrowserRouter,
  RouterProvider as ReactRouterProvider,
} from "react-router-dom";
import Category from "@/pages/Category";
import User from "@/pages/User";
import Hero from "@/pages/Hero";

// Define your routes
const router = createBrowserRouter([
  {
    path: "/",
    element: <Hero />,
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
    path: "/users",
    element: <Layout children={<User />} />,
    errorElement: <ErrorPage />,
  },
  {
    path: "/settings",
    element: <Layout children={<ProductPage />} />,
    errorElement: <ErrorPage />,
  },
]);

export default function RouterProvider() {
  return <ReactRouterProvider router={router} />;
}
