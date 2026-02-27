import { Layout, Menu, Dropdown, Button } from "antd";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  BankOutlined,
  ApiOutlined,
  TeamOutlined,
  BookOutlined,
  LogoutOutlined,
  UserOutlined,
} from "@ant-design/icons";
import "./MainLayout.css";
import { useAuth } from "../../../contexts/AuthContext";

const { Header, Content, Sider } = Layout;

export const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();

  const menuItems = [
    {
      key: "/districts",
      icon: <BankOutlined />,
      label: "Districts",
      onClick: () => navigate("/districts"),
    },
    {
      key: "/schools",
      icon: <BookOutlined />,
      label: "Schools",
      onClick: () => navigate("/schools"),
    },
    {
      key: "/beacons",
      icon: <ApiOutlined />,
      label: "Beacons",
      onClick: () => navigate("/beacons"),
    },
    {
      key: "/faculty",
      icon: <TeamOutlined />,
      label: "Faculty",
      onClick: () => navigate("/faculty"),
    },
  ];

  const selectedKey = "/" + location.pathname.split("/")[1];

  const handleLogout = async () => {
    await logout();
    navigate("/auth/login");
  };

  const userMenuItems = [
    {
      key: "profile",
      icon: <UserOutlined />,
      label: "Profile",
      onClick: () => console.log("Go to profile"),
    },
    {
      key: "logout",
      icon: <LogoutOutlined />,
      label: "Logout",
      onClick: handleLogout,
      danger: true,
    },
  ];

  return (
    <Layout className="main-layout">
      <Header className="main-header" style={{ backgroundColor: "#003380" }}>
        <div className="logo" onClick={() => navigate("/")}>
          LiveFree Schools
        </div>
        <div style={{ marginLeft: "auto" }}>
          <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
            <Button type="text" style={{ color: "#fff" }}>
              <UserOutlined /> {user?.name || user?.email}
            </Button>
          </Dropdown>
        </div>
      </Header>
      <Layout>
        <Sider width={200} className="site-sider">
          <Menu
            mode="inline"
            selectedKeys={[selectedKey]}
            items={menuItems}
            style={{ height: "100%", borderRight: 0 }}
          />
        </Sider>
        <Layout style={{ padding: "0 24px 24px" }}>
          <Content className="main-content">
            <div className="content-wrapper">
              <Outlet />
            </div>
          </Content>
        </Layout>
      </Layout>
    </Layout>
  );
};
