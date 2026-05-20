import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // Proxy para redirigir llamadas /api/* al backend en desarrollo
    // Descomentar y ajustar la URL cuando el API de C# esté lista:
    // proxy: {
    //   "/api": {
    //     target: "http://localhost:5000",
    //     changeOrigin: true,
    //   },
    // },
  },
});