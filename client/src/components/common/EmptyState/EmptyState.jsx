import { Empty, Button, theme } from "antd";
import { InboxOutlined } from "@ant-design/icons";
import "./EmptyState.css";

export const EmptyState = ({
  title = "No data yet",
  description = "There's nothing to show here right now.",
  actionText = null,
  onAction = null,
  icon = InboxOutlined,
}) => {
  const { token } = theme.useToken();
  const IconElement = icon;

  return (
    <Empty
      className="empty-state"
      image={<IconElement style={{ fontSize: 56, color: token.colorPrimary }} />}
      description={
        <div>
          <p className="empty-state__title" style={{ color: token.colorText }}>
            {title}
          </p>
          <p className="empty-state__description" style={{ color: token.colorTextSecondary }}>
            {description}
          </p>
        </div>
      }
    >
      {actionText && onAction && (
        <Button type="primary" onClick={onAction}>
          {actionText}
        </Button>
      )}
    </Empty>
  );
};
