import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { useNotification } from "@/contexts/Notification";
import { LoginForm } from "../components/LoginForm";
import { ROUTES } from "@/router/routes.config";
import { Switch, Tooltip, Typography, theme } from "antd";
import {
  SunOutlined,
  MoonOutlined,
  SafetyCertificateOutlined,
  ThunderboltOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import { useUIStore } from "@/store/ui.store";
import logo from "@/assets/logo-facilityos.png";
import "./LoginPage.css";

const { Title, Paragraph } = Typography;
const { useToken } = theme;

const highlights = [
  { icon: <SafetyCertificateOutlined />, text: "Role-based access across districts and schools" },
  { icon: <ThunderboltOutlined />, text: "Real-time beacon tracking for emergency response" },
  { icon: <TeamOutlined />, text: "Centralized faculty and facility management" },
];

export const LoginPage = () => {
  const uiTheme = useUIStore((state) => state.theme);
  const setTheme = useUIStore((state) => state.setTheme);
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const { showSuccess } = useNotification();
  const { token } = useToken();

  const from = location.state?.from?.pathname || ROUTES.HOME;

  const handleSubmit = async (credentials) => {
    const result = await login(credentials);

    if (result.success) {
      showSuccess("Welcome back!");
      navigate(from, { replace: true });
    }

    return result;
  };

  return (
    <div className="login-page">
      <div className="login-brand-panel">
        <div className="login-brand-content">
          <img src={logo} alt="FacilityOS" className="login-brand-logo" />
          <Title level={2} className="login-brand-title">
            FacilityOS
          </Title>
          <Paragraph className="login-brand-tagline">
            The operations platform for safer, better-connected schools.
          </Paragraph>
          <ul className="login-brand-highlights">
            {highlights.map((item, index) => (
              <li key={index}>
                <span className="login-brand-highlight-icon">{item.icon}</span>
                {item.text}
              </li>
            ))}
          </ul>
        </div>
      </div>

      <div className="login-form-panel" style={{ backgroundColor: token.colorBgLayout }}>
        <div className="theme-toggle">
          <Tooltip title={uiTheme === "dark" ? "Switch to light mode" : "Switch to dark mode"}>
            <Switch
              checked={uiTheme === "dark"}
              onChange={(checked) => setTheme(checked ? "dark" : "light")}
              checkedChildren={<MoonOutlined />}
              unCheckedChildren={<SunOutlined />}
            />
          </Tooltip>
        </div>

        <div className="login-form-inner">
          <div className="login-mobile-brand">
            <img src={logo} alt="FacilityOS" />
            <span style={{ color: token.colorText }}>FacilityOS</span>
          </div>
          <LoginForm onSubmit={handleSubmit} />
        </div>
      </div>
    </div>
  );
};
