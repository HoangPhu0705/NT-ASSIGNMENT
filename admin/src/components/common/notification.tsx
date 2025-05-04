import React from "react";
import { useToaster } from "react-hot-toast/headless";

const Notification: React.FC = () => {
  const { toasts, handlers } = useToaster();
  const { startPause, endPause } = handlers;

  return (
    <div
      style={{
        position: "fixed",
        top: 20,
        right: 20,
        zIndex: 1000,
      }}
      onMouseEnter={startPause}
      onMouseLeave={endPause}
    >
      {toasts
        .filter((toast) => toast.visible)
        .map((toast) => (
          <div
            key={toast.id}
            {...toast.ariaProps}
            style={{
              background: toast.type === "error" ? "#FEE2E2" : "#D1FAE5",
              color: toast.type === "error" ? "#B91C1C" : "#065F46",
              padding: "12px 16px",
              marginBottom: 8,
              borderRadius: 4,
              border:
                toast.type === "error"
                  ? "1px solid #EF4444"
                  : "1px solid #10B981",
              boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
              maxWidth: 350,
              transition: "opacity 0.3s ease",
              opacity: toast.visible ? 1 : 0,
            }}
          >
            {typeof toast.message === "function"
              ? toast.message(toast)
              : toast.message}
          </div>
        ))}
    </div>
  );
};

export default Notification;
