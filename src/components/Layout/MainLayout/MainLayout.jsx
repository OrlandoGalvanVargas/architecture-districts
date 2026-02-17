import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import "./MainLayout.css";

const { Header, Content } = Layout;

export const MainLayout = () => {
  return (
    <Layout className="main-layout">
      <Header className="main-header">
        <div className="logo">LiveFree Schools</div>
        <nav className="main-nav"></nav>
      </Header>
      <Content className="main-content">
        <div className="content-wrapper">
          <Outlet />
        </div>
      </Content>
    </Layout>
  );
};
