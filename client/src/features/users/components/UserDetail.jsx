import { Button, Card, Descriptions, Space, Tag, Popconfirm } from "antd";
import { ArrowLeftOutlined, DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { getEntityTypeLabel } from "../constants/userConstants";
import "./UserDetail.css";

export const UserDetail = ({ user, isDeleting = false, onEdit, onDelete, onBack }) => {
  if (!user) return null;

  return (
    <Card>
      <div className="user-detail-header">
        <Space wrap size={10}>
          <span className="user-detail-name">{user.name}</span>
          <Tag color={user.isActive ? "green" : "red"}>{user.isActive ? "Active" : "Inactive"}</Tag>
        </Space>

        <Space wrap>
          {onBack && (
            <Button icon={<ArrowLeftOutlined />} onClick={onBack} disabled={isDeleting}>
              Back
            </Button>
          )}
          {onEdit && (
            <Button type="primary" icon={<EditOutlined />} onClick={onEdit} disabled={isDeleting}>
              Edit
            </Button>
          )}
          {onDelete && (
            <Popconfirm
              title="Delete user"
              description={`Are you sure you want to delete "${user.name}"?`}
              onConfirm={onDelete}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true, loading: isDeleting }}
            >
              <Button danger icon={<DeleteOutlined />} disabled={isDeleting}>
                Delete
              </Button>
            </Popconfirm>
          )}
        </Space>
      </div>

      <Descriptions bordered column={{ xs: 1, sm: 1, md: 2 }} size="middle">
        <Descriptions.Item label="Full Name" span={2}>
          {user.name}
        </Descriptions.Item>
        <Descriptions.Item label="Email" span={2}>
          {user.email}
        </Descriptions.Item>
        <Descriptions.Item label="Role">
          <Tag
            color={
              user.role === "Admin"
                ? "gold"
                : user.role === "DistrictAdmin"
                  ? "blue"
                  : user.role === "SchoolAdmin"
                    ? "green"
                    : "default"
            }
          >
            {user.role}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="Entity Type">
          {getEntityTypeLabel(user.entityType)}
        </Descriptions.Item>
        <Descriptions.Item label="Entity ID">{user.entityId || "—"}</Descriptions.Item>
        <Descriptions.Item label="Created At">
          {new Date(user.createdAt).toLocaleDateString("en-US", {
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </Descriptions.Item>
        <Descriptions.Item label="Last Updated">
          {user.updatedAt
            ? new Date(user.updatedAt).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })
            : "Never"}
        </Descriptions.Item>
      </Descriptions>
    </Card>
  );
};
