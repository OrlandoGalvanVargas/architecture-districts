import { Layout, Menu } from "antd";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {
  HomeOutlined,
  BankOutlined,
  ApiOutlined,
  TeamOutlined,
  BookOutlined,
} from "@ant-design/icons";
import "./MainLayout.css";

const { Header, Content, Sider } = Layout;

export const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();

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

  return (
    <Layout className="main-layout">
      <Header className="main-header" style={{ backgroundColor: "#003380" }}>
        <div className="logo" onClick={() => navigate("/")}>
          <HomeOutlined style={{ marginRight: 8 }} />
          LiveFree Schools
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
