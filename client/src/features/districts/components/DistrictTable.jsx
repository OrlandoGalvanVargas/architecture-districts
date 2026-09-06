import { Table, Button, Space, Tag, Tooltip, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";

export const DistrictTable = ({
  districts = [],
  loading = false,
  onView = null,
  onEdit = null,
  onDelete = null,
}) => {
  const columns = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
      sorter: (a, b) => a.name.localeCompare(b.name),
      render: (name) => <strong>{name}</strong>,
    },
    {
      title: "Code",
      dataIndex: "code",
      key: "code",
      width: 110,
      render: (code) => <Tag color="blue">{code}</Tag>,
    },
    {
      title: "State",
      dataIndex: "state",
      key: "state",
      width: 80,
      align: "center",
    },
    {
      title: "City",
      dataIndex: "city",
      key: "city",
      width: 150,
    },
    {
      title: "Schools",
      dataIndex: "schoolCount",
      key: "schoolCount",
      width: 90,
      align: "center",
      sorter: (a, b) => a.schoolCount - b.schoolCount,
      render: (count) => count || 0,
    },
    {
      title: "Beacons",
      dataIndex: "beaconCount",
      key: "beaconCount",
      width: 90,
      align: "center",
      render: (count) => count || 0,
    },
    {
      title: "Faculty",
      dataIndex: "facultyCount",
      key: "facultyCount",
      width: 90,
      align: "center",
      render: (count) => count || 0,
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
            <Tooltip title="Edit district">
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
              title="Delete district"
              description={`Are you sure you want to delete "${record.name}"?`}
              onConfirm={() => onDelete(record)}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true }}
            >
              <Tooltip title="Delete district">
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
      dataSource={districts}
      loading={loading}
      rowKey="id"
      size="middle"
      scroll={{ x: 850 }}
      pagination={{
        pageSize: 10,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} districts`,
        pageSizeOptions: [10, 20, 50],
      }}
    />
  );
};
