import { Modal, Descriptions, Tag, Avatar } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { theme as antdTheme } from "antd";
import { useAuth } from "@/contexts/AuthContext";
import { getEntityTypeLabel, getRoleColor } from "@/features/users/constants/userConstants";
import "./ProfileModal.css";

const getInitials = (name = "") =>
  name
    .split(" ")
    .map((part) => part[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();

export const ProfileModal = ({ open, onClose }) => {
  const { user } = useAuth();
  const { token } = antdTheme.useToken();

  if (!user) return null;

  return (
    <Modal
      open={open}
      onCancel={onClose}
      footer={null}
      width={480}
      destroyOnHidden
      closable={false}
    >
      <div className="profile-modal-header">
        <Avatar
          size={56}
          style={{ backgroundColor: token.colorPrimaryBg, color: token.colorPrimary }}
        >
          {getInitials(user.name) || <UserOutlined />}
        </Avatar>
        <div className="profile-modal-identity">
          <span className="profile-modal-name">{user.name}</span>
          <span className="profile-modal-email">{user.email}</span>
        </div>
        <Tag color={getRoleColor(user.role)} className="profile-modal-role">
          {user.role}
        </Tag>
      </div>

      <Descriptions bordered column={1} size="middle">
        <Descriptions.Item label="Entity Type">
          {getEntityTypeLabel(user.entityType)}
        </Descriptions.Item>
        <Descriptions.Item label="Entity ID">{user.entityId || "—"}</Descriptions.Item>
      </Descriptions>
    </Modal>
  );
};
