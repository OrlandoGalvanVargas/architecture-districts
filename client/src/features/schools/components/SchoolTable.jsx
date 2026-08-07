import { Table, Button, Tag, Space, Select, Switch, Popconfirm } from "antd";
import {
  PlusOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

const { Option } = Select;

export const SchoolTable = ({
  schools = [],
  loading = false,
  onView = null,
  onEdit = null,
  onDelete = null,
}) => {
  const columns = [
    { title: "Name", dataIndex: "name", key: "name" },
    {
      title: "Code",
      dataIndex: "schoolCode",
      key: "schoolCode",
      render: (code) => <Tag color="blue">{code}</Tag>,
    },
    {
      title: "Level",
      dataIndex: "level",
      key: "level",
      render: (level) => <Tag color="purple">{level}</Tag>,
    },
    {
      title: "Type",
      dataIndex: "type",
      key: "type",
      render: (type) => <Tag>{type}</Tag>,
    },
    { title: "District", dataIndex: "districtName", key: "districtName" },
    { title: "Capacity", dataIndex: "studentCapacity", key: "studentCapacity" },
    {
      title: "Status",
      dataIndex: "isActive",
      key: "isActive",
      render: (active) => (
        <Tag color={active ? "green" : "red"}>
          {active ? "Active" : "Inactive"}
        </Tag>
      ),
    },
    {
      title: "Actions",
      key: "actions",
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
              title="Delete school"
              description="Are you sure you want to delete this school?"
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
      dataSource={schools}
      columns={columns}
      rowKey="id"
      loading={loading}
      pagination={{
        pageSize: 10,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} schools`,
      }}
    />
  );
};
