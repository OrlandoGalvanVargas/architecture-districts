import { Breadcrumb, Typography, theme } from "antd";
import { Link } from "react-router-dom";
import { HomeOutlined } from "@ant-design/icons";
import "./PageHeader.css";

const { Title } = Typography;

export const PageHeader = ({ title, subtitle = null, breadcrumbs = [], extra = null }) => {
  const { token } = theme.useToken();

  const breadcrumbItems = [
    {
      title: (
        <Link to="/">
          <HomeOutlined />
        </Link>
      ),
    },
    ...breadcrumbs.map((crumb, index) => {
      const isLast = index === breadcrumbs.length - 1;
      return { title: isLast ? crumb.label : <Link to={crumb.path}>{crumb.label}</Link> };
    }),
  ];

  return (
    <div
      className="page-header"
      style={{ backgroundColor: token.colorBgContainer, borderColor: token.colorBorderSecondary }}
    >
      <div className="page-header-content">
        <div className="page-header-main">
          <Breadcrumb items={breadcrumbItems} />
          <Title level={2} className="page-title" style={{ color: token.colorText }}>
            {title}
          </Title>
          {subtitle && (
            <p className="page-subtitle" style={{ color: token.colorTextSecondary }}>
              {subtitle}
            </p>
          )}
        </div>
        {extra && <div className="page-header-extra">{extra}</div>}
      </div>
    </div>
  );
};
