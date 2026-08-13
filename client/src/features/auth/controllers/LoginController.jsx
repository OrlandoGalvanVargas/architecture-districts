import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { LoginForm } from "../components/LoginForm";
import { RoutePaths } from "@/router/RoutePaths";

export const LoginController = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const from = location.state?.from?.pathname || RoutePaths.districts.list();

  const handleSubmit = async (credentials) => {
    setLoading(true);
    setError(null);

    const result = await login(credentials);

    if (result.success) {
      navigate(from, { replace: true });
    } else {
      setError(result.error);
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        background: "#f0f2f5",
      }}
    >
      <LoginForm onSubmit={handleSubmit} loading={loading} error={error} />
    </div>
  );
};
