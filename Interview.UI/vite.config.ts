import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(({ mode }) => {
  const environment = loadEnv(mode, ".", "");

  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: {
        "/products": {
          target: environment.VITE_API_TARGET || "http://localhost:8080",
          changeOrigin: true,
        },
      },
    },
  };
});
