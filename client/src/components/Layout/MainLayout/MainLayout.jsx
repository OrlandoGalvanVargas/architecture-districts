import {
  Layout,
  Menu,
  Dropdown,
  Button,
  Avatar,
  Space,
  Alert,
  Switch,
  Tooltip,
  Drawer,
  Grid,
  theme,
} from "antd";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  BankOutlined,
  ApiOutlined,
  TeamOutlined,
  BookOutlined,
  UserOutlined,
  LogoutOutlined,
  SunOutlined,
  MoonOutlined,
  DashboardOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  MenuOutlined,
} from "@ant-design/icons";
import { useMemo, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { ROUTES } from "@/router/routes.config";
import { useUIStore } from "@/store/ui.store";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ProfileModal } from "@/components/common/ProfileModal/ProfileModal";
import { ConfirmDialog } from "@/components/common/ConfirmDialog/ConfirmDialog";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { ErrorBoundary } from "@/components/common/ErrorBoundary/ErrorBoundary";
import { useOnlineStatus } from "@/hooks/useOnlineStatus";
import logo from "@/assets/logo-facilityos.png";
import "./MainLayout.css";

const { Header, Content, Sider } = Layout;
const { useBreakpoint } = Grid;

export const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = theme.useToken();
  const { user, logout } = useAuth();
  const { sidebarCollapsed, toggleSidebar } = useUIStore();
  const themeMode = useUIStore((state) => state.theme);
  const setTheme = useUIStore((state) => state.setTheme);
  const { can } = usePermission();
  const isOnline = useOnlineStatus();
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [profileOpen, setProfileOpen] = useState(false);
  const [logoutConfirmOpen, setLogoutConfirmOpen] = useState(false);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const menuItems = useMemo(() => {
    const items = [{ key: ROUTES.HOME, icon: <DashboardOutlined />, label: "Home" }];

    if (can(PERMISSIONS.DISTRICTS.VIEW_LIST)) {
      items.push({ key: ROUTES.DISTRICTS.LIST, icon: <BankOutlined />, label: "Districts" });
    }
    if (can(PERMISSIONS.SCHOOLS.VIEW_LIST)) {
      items.push({ key: ROUTES.SCHOOLS.LIST, icon: <BookOutlined />, label: "Schools" });
    }
    if (can(PERMISSIONS.USERS.VIEW_LIST)) {
      items.push({ key: ROUTES.USERS.LIST, icon: <UserOutlined />, label: "Users" });
    }
    if (can(PERMISSIONS.BEACONS.VIEW_LIST)) {
      items.push({ key: ROUTES.BEACONS.LIST, icon: <ApiOutlined />, label: "Beacons" });
    }
    if (can(PERMISSIONS.FACULTIES.VIEW_LIST)) {
      items.push({ key: ROUTES.FACULTIES.LIST, icon: <TeamOutlined />, label: "Faculties" });
    }

    return items;
  }, [can]);

  const selectedKey = useMemo(() => {
    const path = location.pathname;
    const match = Object.values(ROUTES).find((routes) => {
      if (typeof routes === "object" && routes.LIST) {
        return path.startsWith(routes.LIST);
      }
      return false;
    });
    return match?.LIST || ROUTES.HOME;
  }, [location.pathname]);

  const handleMenuClick = ({ key }) => {
    setMobileMenuOpen(false);
    navigate(key);
  };

  const handleLogout = async () => {
    await logout();
    navigate(ROUTES.AUTH.LOGIN);
  };

  const handleTriggerClick = () => {
    if (isMobile) {
      setMobileMenuOpen(true);
    } else {
      toggleSidebar();
    }
  };

  const userMenuItems = [
    {
      key: "profile",
      icon: <UserOutlined />,
      label: "Profile",
      onClick: () => setProfileOpen(true),
    },
    { type: "divider" },
    {
      key: "logout",
      icon: <LogoutOutlined />,
      label: "Logout",
      onClick: () => setLogoutConfirmOpen(true),
      danger: true,
    },
  ];

  const getUserDisplayName = () => user?.name || user?.email || "User";
  const getUserInitial = () => getUserDisplayName().charAt(0).toUpperCase();

  return (
    <Layout className="main-layout">
      <Header className="main-header">
        <div
          className={`brand-zone ${sidebarCollapsed && !isMobile ? "brand-zone--collapsed" : ""}`}
          onClick={() => {
            setMobileMenuOpen(false);
            navigate(ROUTES.HOME);
          }}
        >
          <img src={logo} alt="FacilityOS" className="brand-logo" />
          {(!sidebarCollapsed || isMobile) && <span className="brand-name">FacilityOS</span>}
        </div>

        <Button
          type="text"
          className="collapse-trigger"
          icon={
            isMobile ? (
              <MenuOutlined />
            ) : sidebarCollapsed ? (
              <MenuUnfoldOutlined />
            ) : (
              <MenuFoldOutlined />
            )
          }
          onClick={handleTriggerClick}
        />

        <div className="header-actions">
          <Tooltip title={themeMode === "dark" ? "Switch to light mode" : "Switch to dark mode"}>
            <Switch
              checked={themeMode === "dark"}
              onChange={(checked) => setTheme(checked ? "dark" : "light")}
              checkedChildren={<MoonOutlined />}
              unCheckedChildren={<SunOutlined />}
            />
          </Tooltip>

          <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
            <Button type="text" className="user-button">
              <Space>
                <Avatar size="small" style={{ backgroundColor: token.colorPrimary, color: "#fff" }}>
                  {getUserInitial()}
                </Avatar>
                <span className="user-name">{getUserDisplayName()}</span>
              </Space>
            </Button>
          </Dropdown>
        </div>
      </Header>

      <Layout>
        {!isMobile && (
          <Sider
            width={248}
            collapsedWidth={80}
            collapsed={sidebarCollapsed}
            trigger={null}
            theme="dark"
            className="site-sider"
          >
            <Menu
              mode="inline"
              theme="dark"
              selectedKeys={[selectedKey]}
              items={menuItems}
              onClick={handleMenuClick}
              className="site-menu"
            />
          </Sider>
        )}

        <Layout
          className={`main-content-layout ${
            !isMobile && sidebarCollapsed ? "main-content-layout--collapsed" : ""
          } ${isMobile ? "main-content-layout--mobile" : ""}`}
        >
          <Content className="main-content">
            <div className="content-wrapper">
              {!isOnline && (
                <Alert
                  type="warning"
                  banner
                  message="You are currently offline. Some features may be unavailable."
                  style={{ marginBottom: 16 }}
                />
              )}
              <ErrorBoundary
                fallback={
                  <ErrorMessage
                    error={{ message: "This page failed to load. Please try again." }}
                    onRetry={() => window.location.reload()}
                  />
                }
              >
                <Outlet />
              </ErrorBoundary>
            </div>
          </Content>
        </Layout>
      </Layout>

      <Drawer
        placement="left"
        open={mobileMenuOpen}
        onClose={() => setMobileMenuOpen(false)}
        closable={false}
        width={260}
        className="mobile-nav-drawer"
        styles={{ body: { padding: 0 } }}
      >
        <div className="mobile-drawer-brand">
          <img src={logo} alt="FacilityOS" className="brand-logo" />
          <span className="brand-name">FacilityOS</span>
        </div>
        <Menu
          mode="inline"
          theme="dark"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={handleMenuClick}
          className="site-menu"
        />
      </Drawer>

      <ProfileModal open={profileOpen} onClose={() => setProfileOpen(false)} />

      <ConfirmDialog
        open={logoutConfirmOpen}
        title="Confirm Logout"
        description="Are you sure you want to log out?"
        confirmText="Logout"
        cancelText="Cancel"
        danger
        loading={false}
        onConfirm={() => {
          setLogoutConfirmOpen(false);
          handleLogout();
        }}
        onCancel={() => setLogoutConfirmOpen(false)}
      />
    </Layout>
  );
};
