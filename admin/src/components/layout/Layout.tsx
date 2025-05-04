import React from "react";
import { SidebarProvider, SidebarTrigger } from "../ui/sidebar";
import { AppSidebar } from "../common/app-side-bar";
import { useAuth } from "oidc-react";
import { DoorOpenIcon } from "lucide-react";
import { Toaster } from "react-hot-toast";
import { Button } from "@/components/ui/button";
import { SearchForm } from "../common/search-form";

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
        <Toaster />{" "}
        <div className="flex w-full justify-between items-center p-4">
          <div className="flex w-1/2 items-center space-x-4">
            <SidebarTrigger />
            <SearchForm className="w-full" />
          </div>
          <Button
            onClick={handleLogout}
            size={"lg"}
            className="bg-transparent hover:cursor-pointer hover:bg-transparent hover:text-white border-1"
          >
            <DoorOpenIcon className="text-black text-2xl" />
          </Button>
        </div>
        {children}
      </main>
    </SidebarProvider>
  );
}
