import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";

// Renderizamos la aplicación React en el elemento con id "root"
createRoot(document.getElementById("root")).render(
  <StrictMode>
    <App />
  </StrictMode>
);