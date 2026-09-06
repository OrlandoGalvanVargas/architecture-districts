import { Table, Button, Space, Tag, Tooltip, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";
import { getBeaconTypeLabel, getBeaconStatusLabel } from "../constants/beaconConstants";

export const BeaconTable = ({
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
      title: "Device Name",
      dataIndex: "deviceName",
      key: "deviceName",
      width: 180,
      ellipsis: true,
      render: (name) => (
        <Tooltip title={name}>
          <strong>{name}</strong>
        </Tooltip>
      ),
    },
    {
      title: "Serial Number",
      dataIndex: "serialNumber",
      key: "serialNumber",
      width: 150,
      render: (serial) => <Tag color="blue">{serial}</Tag>,
    },
    {
      title: "Type",
      dataIndex: "type",
      key: "type",
      width: 120,
      render: (type) => getBeaconTypeLabel(type),
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      width: 120,
      render: (status) => {
        const color =
          status === 1 ? "green" : status === 2 ? "blue" : status === 3 ? "orange" : "red";
        return <Tag color={color}>{getBeaconStatusLabel(status)}</Tag>;
      },
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
      title: "Faculty",
      dataIndex: "facultyName",
      key: "facultyName",
      width: 150,
      render: (name) => name || "—",
    },
    {
      title: "Assigned",
      dataIndex: "isAssigned",
      key: "isAssigned",
      width: 100,
      align: "center",
      render: (isAssigned) =>
        isAssigned ? <Tag color="green">Yes</Tag> : <Tag color="red">No</Tag>,
    },
    {
      title: "Created",
      dataIndex: "createdAt",
      key: "createdAt",
      width: 130,
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
            <Tooltip title="Edit beacon">
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
              title="Delete beacon"
              description={`Are you sure you want to delete "${record.deviceName}"?`}
              onConfirm={() => onDelete(record)}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true }}
            >
              <Tooltip title="Delete beacon">
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
      scroll={{ x: 1380 }}
      pagination={{
        current: pagination.page || 1,
        pageSize: pagination.pageSize || 10,
        total: pagination.totalCount || 0,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} beacons`,
        onChange: (page, pageSize) => {
          onPageChange?.(page, pageSize);
        },
      }}
    />
  );
};
