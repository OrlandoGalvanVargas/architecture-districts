import { Table, Button, Space, Tag, Tooltip, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";
import { getEntityTypeLabel } from "../constants/userConstants";

export const UserTable = ({
  data = [],
  loading = false,
  pagination = {},
  onPageChange,
  onView,
  onEdit,
  onDelete,
}) => {
  const columns = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
      width: 180,
      ellipsis: true,
      render: (name) => (
        <Tooltip title={name}>
          <strong>{name}</strong>
        </Tooltip>
      ),
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      width: 200,
      ellipsis: true,
      render: (email) => <Tooltip title={email}>{email}</Tooltip>,
    },
    {
      title: "Role",
      dataIndex: "role",
      key: "role",
      width: 140,
      render: (role) => (
        <Tag
          color={
            role === "Admin"
              ? "gold"
              : role === "DistrictAdmin"
                ? "blue"
                : role === "SchoolAdmin"
                  ? "green"
                  : "default"
          }
        >
          {role}
        </Tag>
      ),
    },
    {
      title: "Entity Type",
      dataIndex: "entityType",
      key: "entityType",
      width: 120,
      render: (type) => getEntityTypeLabel(type),
    },
    {
      title: "Entity ID",
      dataIndex: "entityId",
      key: "entityId",
      width: 100,
      align: "center",
      render: (entityId) => entityId || "—",
    },
    {
      title: "Status",
      dataIndex: "isActive",
      key: "isActive",
      width: 100,
      align: "center",
      render: (isActive) =>
        isActive ? <Tag color="green">Active</Tag> : <Tag color="red">Inactive</Tag>,
    },
    {
      title: "Created",
      dataIndex: "createdAt",
      key: "createdAt",
      width: 160,
      render: (date) => (date ? new Date(date).toLocaleDateString("en-US") : "—"),
    },
    {
      title: "",
      key: "actions",
      width: 120,
      fixed: "right",
      render: (_, record) => (
        <Space size={4}>
          {onView && (
            <Tooltip title="View details">
              <Button
                type="text"
                icon={<EyeOutlined />}
                onClick={() => onView(record)}
                size="small"
              />
            </Tooltip>
          )}
          {onEdit && (
            <Tooltip title="Edit user">
              <Button
                type="text"
                icon={<EditOutlined />}
                onClick={() => onEdit(record)}
                size="small"
              />
            </Tooltip>
          )}
          {onDelete && (
            <Popconfirm
              title="Delete user"
              description={`Are you sure you want to delete "${record.name}"?`}
              onConfirm={() => onDelete(record)}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true }}
            >
              <Tooltip title="Delete user">
                <Button type="text" danger icon={<DeleteOutlined />} size="small" />
              </Tooltip>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={data}
      loading={loading}
      rowKey="id"
      size="middle"
      scroll={{ x: 1200 }}
      pagination={{
        current: pagination.page || 1,
        pageSize: pagination.pageSize || 10,
        total: pagination.totalCount || 0,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} users`,
        onChange: (page, pageSize) => {
          onPageChange?.(page, pageSize);
        },
      }}
    />
  );
};
