// App.jsx: Componente principal de la aplicación 
// React para el panel de administración de TECAir

import { useState } from "react";
import LoginPage from "./pages/LoginPage";
import Dashboard from "./pages/Dashboard";

// Componente principal de la aplicación
export default function App() {
  const [usuario, setUsuario] = useState(() => {
    try {
      const guardado = sessionStorage.getItem("tecair_usuario");
      return guardado ? JSON.parse(guardado) : null;
    } catch {
      return null;
    }
  });

  // Función para manejar el éxito del login, guardando el usuario y token en sessionStorage
  const handleLoginSuccess = (usuarioData, token) => {
    sessionStorage.setItem("tecair_usuario", JSON.stringify(usuarioData));
    sessionStorage.setItem("tecair_token", token);
    setUsuario(usuarioData);
  };

  // Función para manejar el logout, limpiando sessionStorage y estado de usuario
  const handleLogout = () => {
    sessionStorage.removeItem("tecair_usuario");
    sessionStorage.removeItem("tecair_token");
    setUsuario(null);
  };

  // Renderizamos el Dashboard si el usuario está autenticado, de lo contrario mostramos la página de login
  return usuario ? (
    <Dashboard usuario={usuario} onLogout={handleLogout} />
  ) : (
    <LoginPage onLoginSuccess={handleLoginSuccess} />
  );
}