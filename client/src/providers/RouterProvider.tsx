import ErrorPage from "@/components/ui/error_page";
import Login from "@/pages/auth/Login";
import HomePage from "@/pages/home/HomePage";
import {
  createBrowserRouter,
  RouterProvider as ReactRouterProvider,
} from "react-router-dom";

// Define your routes
const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
    errorElement: <ErrorPage />,
  },
  {
    path: "auth/login",
    element: <Login />,
    errorElement: <ErrorPage />,
  },
]);

export default function RouterProvider() {
  return <ReactRouterProvider router={router} />;
}
