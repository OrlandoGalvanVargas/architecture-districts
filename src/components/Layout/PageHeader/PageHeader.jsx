import { Breadcrumb } from "antd";
import { Link } from "react-router-dom";
import "./PageHeader.css";

export const PageHader = ({ title, breadcrumbs = [], extra = null }) => {
  const breadcrumbItems = breadcrumbs.map((crumb, index) => {
    const isLast = index === breadcrumbs.length - 1;

    return {
      title: isLast ? crumb.label : <Link to={crumb.path}>{crumb.label}</Link>,
    };
  });

  return (
    <div className="page-header">
      <div className="page-header-content">
        <div className="page-header-main">
          <Breadcrumb items={breadcrumbItems} />
          <h1 className="page-title">{title}</h1>
        </div>
        {extra && <div className="page-header-extra">{extra}</div>}
      </div>
    </div>
  );
};
