import Layout from "@/components/layout/Layout";
import Login from "@/pages/auth/Login";
import Dashboard from "@/pages/dashboard/Dashboard";
import {
  createBrowserRouter,
  RouterProvider as ReactRouterProvider,
} from "react-router-dom";

// Define your routes
const router = createBrowserRouter([
  {
    path: "auth/login",
    element: <Login />,
  },
  {
    path: "/",
    element: <Layout children={<Dashboard />} />,
  },
]);

export default function RouterProvider() {
  return <ReactRouterProvider router={router} />;
}
