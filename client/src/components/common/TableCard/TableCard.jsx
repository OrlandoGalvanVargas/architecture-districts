import { theme } from "antd";
import "./TableCard.css";

export const TableCard = ({ children }) => {
  const { token } = theme.useToken();

  return (
    <div
      className="table-card"
      style={{
        backgroundColor: token.colorBgContainer,
        borderColor: token.colorBorderSecondary,
        borderRadius: token.borderRadiusLG,
      }}
    >
      {children}
    </div>
  );
};
