/* eslint-disable @typescript-eslint/no-explicit-any */
import { Sheet, SheetTrigger, SheetContent } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import {
  NavigationMenu,
  NavigationMenuList,
  NavigationMenuLink,
} from "@/components/ui/navigation-menu";
import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <header className="flex h-20 w-full shrink-0 items-center px-4 md:px-6">
      <Sheet>
        <SheetTrigger asChild>
          <Button variant="outline" size="icon" className="lg:hidden">
            <MenuIcon className="h-6 w-6" />
          </Button>
        </SheetTrigger>
        <SheetContent side="left">
          <Link to="/">
            <ShirtIcon className="h-6 w-6" />
            <span className="sr-only">ShadCN</span>
          </Link>
          <div className="grid gap-2 py-6">
            <Link
              to="/"
              className="flex w-full items-center py-2 text-md font-semibold"
            >
              Home
            </Link>
            <Link
              to="/about"
              className="flex w-full items-center py-2 text-md font-semibold"
            >
              About
            </Link>
            <Link
              to="/services"
              className="flex w-full items-center py-2 text-md font-semibold"
            >
              Services
            </Link>
            <Link
              to="/portfolio"
              className="flex w-full items-center py-2 text-md font-semibold"
            >
              Portfolio
            </Link>
            <Link
              to="/contact"
              className="flex w-full items-center py-2 text-md font-semibold"
            >
              Contact
            </Link>
          </div>
        </SheetContent>
      </Sheet>

      <Link to="/" className="mr-6 hidden lg:flex">
        <ShirtIcon className="h-6 w-6" />
      </Link>
      <NavigationMenu className="hidden lg:flex">
        <NavigationMenuList>
          <NavigationMenuLink asChild className="text-xl ">
            <Link
              to="/shop"
              className="group inline-flex h-9 w-max items-center justify-center rounded-md bg-white px-4 py-2"
            >
              Shop
            </Link>
          </NavigationMenuLink>
          <NavigationMenuLink asChild className="text-xl">
            <Link
              to="/planet-and-people"
              className="group inline-flex h-9 w-max items-center justify-center rounded-md bg-white px-4 py-2"
            >
              Planet & People
            </Link>
          </NavigationMenuLink>
          <NavigationMenuLink asChild className="text-xl">
            <Link
              to="/softwares"
              className="group inline-flex h-9 w-max items-center justify-center rounded-md bg-white px-4 py-2"
            >
              Softwares
            </Link>
          </NavigationMenuLink>
          <NavigationMenuLink asChild className="text-xl">
            <Link
              to="/Deals"
              className="group inline-flex h-9 w-max items-center justify-center rounded-md bg-white px-4 py-2"
            >
              Deals
            </Link>
          </NavigationMenuLink>
        </NavigationMenuList>
      </NavigationMenu>
      <div className="ml-auto flex gap-2"></div>
    </header>
  );
}

function MenuIcon(props: any) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <line x1="4" x2="20" y1="12" y2="12" />
      <line x1="4" x2="20" y1="6" y2="6" />
      <line x1="4" x2="20" y1="18" y2="18" />
    </svg>
  );
}

function ShirtIcon(props: any) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M20.38 3.46 16 2a4 4 0 0 1-8 0L3.62 3.46a2 2 0 0 0-1.34 2.23l.58 3.47a1 1 0 0 0 .99.84H6v10c0 1.1.9 2 2 2h8a2 2 0 0 0 2-2V10h2.15a1 1 0 0 0 .99-.84l.58-3.47a2 2 0 0 0-1.34-2.23z" />
    </svg>
  );
}
