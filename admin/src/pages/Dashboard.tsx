import { ChartAreaInteractive } from "@/components/common/chart-area-interactive";
import { SectionCards } from "@/components/common/section-cards";
import React from "react";
import { toast } from "react-hot-toast";

const Dashboard: React.FC = () => {
  const notify = () => toast("Here is your toast.");

  return (
    <div className="flex flex-1 flex-col">
      <div className="@container/main flex flex-1 flex-col gap-2">
        <div className="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
          <SectionCards />
          <div className="px-4 lg:px-6">
            <ChartAreaInteractive />
          </div>
          <button onClick={notify}>Make me a toast</button>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
