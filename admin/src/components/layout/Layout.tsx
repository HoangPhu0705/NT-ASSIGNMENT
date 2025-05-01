import React from "react";
import { SidebarProvider, SidebarTrigger } from "../ui/sidebar";
import { AppSidebar } from "../common/AppSideBar";
import { useAuth } from "oidc-react";

export default function Layout({ children }: { children: React.ReactNode }) {
  const auth = useAuth();

  const handleLogout = () => {
    auth.signOutRedirect({
      post_logout_redirect_uri: "https://localhost:5173/signout-callback-oidc",
    });
  };

  return (
    <SidebarProvider>
      <AppSidebar />
      <main className="w-full min-h-screen">
        <div className="flex justify-between items-center p-4">
          <SidebarTrigger />
          <button
            onClick={handleLogout}
            className="btn btn-primary hover:cursor-pointer"
          >
            Logout
          </button>
        </div>
        {children}
      </main>
    </SidebarProvider>
  );
}
