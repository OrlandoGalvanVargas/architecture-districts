import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import path from "path";

export default defineConfig(({ mode }) => {
  // Carga las variables del archivo .env
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [react()],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    server: {
      port: 3000,
      open: false,
      cors: true,
      proxy: {
        "/api": {
          target: env.VITE_API_URL || "http://localhost:5000",
          changeOrigin: true,
          secure: false,
          cookieDomainRewrite: "localhost", // Asegura que la cookie se guarde
        },
      },
    },
    build: {
      outDir: "dist",
      sourcemap: false,
      minify: "esbuild",
      chunkSizeWarningLimit: 1000,
      rollupOptions: {
        output: {
          manualChunks: {
            vendor: ["react", "react-dom", "react-router-dom"],
            antd: ["antd", "@ant-design/icons"],
            http: ["axios"],
            state: ["zustand"],
          },
        },
      },
    },
    test: {
      globals: true,
      environment: "jsdom",
      setupFiles: "./src/test/setup.js",
      css: true,
      coverage: {
        provider: "v8",
        reporter: ["text", "json", "html"],
        exclude: ["node_modules/", "src/test/"],
      },
    },
  };
});
