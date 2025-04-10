import { Button } from "@/components/ui/button";
import Navbar from "@/components/ui/navbar";
import React from "react";

function HomePage() {
  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <main className="container mx-auto py-8">
        <section className="text-center">
          <h2 className="text-3xl font-semibold text-foreground mb-4">
            Welcome to Nextech
          </h2>
          <p className="text-muted-foreground mb-6">
            Explore our latest tech products!
          </p>
          <Button className="bg-primary text-primary-foreground">
            Shop Now
          </Button>
        </section>
      </main>
    </div>
  );
}

export default HomePage;
