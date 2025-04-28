import React from "react";
import { SidebarProvider, SidebarTrigger } from "../ui/sidebar";
import { AppSidebar } from "../common/AppSideBar";

export default function Layout({ children }: { children: React.ReactNode }) {
  // const auth = useAuth();
  // const navigate = useNavigate();

  // const handleLogout = () => {
  //   auth.signOutRedirect({
  //     post_logout_redirect_uri: "http://localhost:5173/signout-callback-oidc",
  //   });
  // };

  return (
    <SidebarProvider>
      <AppSidebar />
      <main>
        <SidebarTrigger />
        {children}
      </main>
    </SidebarProvider>
  );
}
