import { Table, Button, Space, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";

export const DistrictTable = ({
  districst = [],
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
    },
    {
      title: "Code",
      dataIndex: "code",
      key: "code",
      width: 120,
    },
    {
      title: "State",
      dataIndex: "state",
      key: "state",
      width: 100,
    },
    {
      title: "City",
      dataIndex: "city",
      key: "city",
    },
    {
      title: "Schools",
      dataIndex: "schoolCount",
      key: "schoolCount",
      width: 100,
      align: "center",
      render: (count) => count || 0,
    },
    {
      title: "Actions",
      key: "actions",
      width: 150,
      fixed: "right",
      render: (_, record) => (
        <Space size="small">
          {onView && (
            <Button
              type="link"
              icon={<EyeOutlined />}
              onClick={() => onView(record)}
              size="small"
            >
              View
            </Button>
          )}
          {onEdit && (
            <Button
              type="link"
              icon={<EditOutlined />}
              onClick={() => onEdit(record)}
              size="small"
            >
              Edit
            </Button>
          )}
          {onDelete && (
            <Popconfirm
              title="Delete district"
              description="Are you sure you want to delete this district?"
              onConfirm={() => onDelete(record)}
              okText="Yes"
              cancelText="No"
            >
              <Button type="link" danger icon={<DeleteOutlined />} size="small">
                Delete
              </Button>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={districst}
      loading={loading}
      rowKey="id"
      pagination={{
        pageSize: 10,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} districts`,
      }}
    />
  );
};
