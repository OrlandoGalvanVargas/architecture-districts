/* eslint-disable react-refresh/only-export-components */
import { StrictMode, useEffect, useMemo } from "react";
import { createRoot } from "react-dom/client";
import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { ConfigProvider, Empty, theme } from "antd";
import { NotificationProvider } from "@/contexts/Notification";
import { queryClient } from "@/config/queryClient";
import { buildThemeConfig } from "@/config/theme/index";
import { useUIStore } from "@/store/ui.store";
import { EmptyState } from "@/components/common/EmptyState/EmptyState";
import App from "./App";
import "./index.css";

const BodyThemeSync = () => {
  const { token } = theme.useToken();

  useEffect(() => {
    document.body.style.backgroundColor = token.colorBgLayout;
    document.body.style.color = token.colorText;
  }, [token.colorBgLayout, token.colorText]);

  return null;
};

const ThemedApp = () => {
  const themeMode = useUIStore((state) => state.theme);

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", themeMode);
  }, [themeMode]);

  const themeConfig = useMemo(() => buildThemeConfig(themeMode), [themeMode]);

  return (
    <ConfigProvider
      theme={themeConfig}
      renderEmpty={(componentName) =>
        componentName === "Table" ? (
          <EmptyState
            title="No records found"
            description="Try adjusting your filters, or create a new record to get started."
          />
        ) : (
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
        )
      }
    >
      <BodyThemeSync />
      <App />
    </ConfigProvider>
  );
};

async function enableMocking() {
  if (import.meta.env.VITE_ENABLE_MOCK !== "true") return;
  const { worker } = await import("./mocks/browser");
  await worker.start({ onUnhandledRequest: "bypass" });
}

enableMocking().then(() => {
  createRoot(document.getElementById("root")).render(
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <NotificationProvider>
          <ThemedApp />
          <ReactQueryDevtools initialIsOpen={false} />
        </NotificationProvider>
      </QueryClientProvider>
    </StrictMode>
  );
});
