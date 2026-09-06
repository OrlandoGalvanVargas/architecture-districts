import { Table, Button, Space, Tag, Tooltip, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";

export const FacultyTable = ({
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
      title: "Full Name",
      dataIndex: "fullName",
      key: "fullName",
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
      title: "Title",
      dataIndex: "title",
      key: "title",
      width: 150,
    },
    {
      title: "Department",
      dataIndex: "department",
      key: "department",
      width: 150,
    },
    {
      title: "District",
      dataIndex: "districtName",
      key: "districtName",
      width: 150,
      render: (name) => name || "—",
    },
    {
      title: "School",
      dataIndex: "schoolName",
      key: "schoolName",
      width: 150,
      render: (name) => name || "—",
    },
    {
      title: "Beacon",
      dataIndex: "beaconDeviceName",
      key: "beaconDeviceName",
      width: 150,
      render: (name, record) =>
        record.beaconId ? (
          <Tooltip title={`Serial: ${record.beaconSerialNumber || "N/A"}`}>
            <Tag color="blue">{name || "Assigned"}</Tag>
          </Tooltip>
        ) : (
          <Tag>None</Tag>
        ),
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
            <Tooltip title="Edit faculty">
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
              title="Delete faculty"
              description={`Are you sure you want to delete "${record.fullName}"?`}
              onConfirm={() => onDelete(record)}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true }}
            >
              <Tooltip title="Delete faculty">
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
      scroll={{ x: 1350 }}
      pagination={{
        current: pagination.page || 1,
        pageSize: pagination.pageSize || 10,
        total: pagination.totalCount || 0,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} faculty`,
        onChange: (page, pageSize) => {
          onPageChange?.(page, pageSize);
        },
      }}
    />
  );
};
