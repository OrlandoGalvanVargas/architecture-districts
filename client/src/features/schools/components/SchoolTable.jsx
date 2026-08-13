import {
  Table,
  Button,
  Tag,
  Space,
  Select,
  Switch,
  Popconfirm,
  Input,
} from "antd";
import {
  PlusOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  ReloadOutlined,
} from "@ant-design/icons";

const { Option } = Select;
const { Search } = Input;

export const SchoolTable = ({
  schools = [],
  loading = false,
  filters = {},
  pagination,
  onPageChange,
  onFilterChange,
  onView,
  onEdit,
  onDelete,
  onCreate,
  onRefresh,
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
      sorter: (a, b) => (a.isActive === b.isActive ? 0 : a.isActive ? -1 : 1),
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
    <div>
      <div style={{ marginBottom: 16 }}>
        <Space style={{ width: "100%", justifyContent: "space-between" }} wrap>
          <Space wrap>
            <Search
              placeholder="Search by name or code..."
              allowClear
              style={{ width: 260 }}
              value={filters.search}
              onChange={(e) => {
                if (!e.target.value) onFilterChange({ search: undefined });
              }}
              onSearch={(value) =>
                onFilterChange({ search: value.trim() || undefined })
              }
              disabled={loading}
            />
            <Select
              placeholder="Level"
              allowClear
              style={{ width: 130 }}
              value={filters.level}
              onChange={(val) => onFilterChange({ level: val || undefined })}
              disabled={loading}
            >
              {["Elementary", "Middle", "High", "K12", "Prek"].map((l) => (
                <Option key={l} value={l}>
                  {l}
                </Option>
              ))}
            </Select>
            <Select
              placeholder="Type"
              allowClear
              style={{ width: 130 }}
              value={filters.type}
              onChange={(val) => onFilterChange({ type: val || undefined })}
              disabled={loading}
            >
              {["Public", "Charter", "Magnet", "Alternative"].map((t) => (
                <Option key={t} value={t}>
                  {t}
                </Option>
              ))}
            </Select>
            <Switch
              checkedChildren="Active"
              unCheckedChildren="All"
              checked={filters.isActive === true}
              onChange={(checked) =>
                onFilterChange({ isActive: checked ? true : undefined })
              }
              disabled={loading}
            />
          </Space>

          <Space>
            <Button
              icon={<ReloadOutlined />}
              onClick={onRefresh}
              disabled={loading}
            >
              Refresh
            </Button>
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={onCreate}
              disabled={loading}
            >
              Create School
            </Button>
          </Space>
        </Space>
      </div>

      <Table
        dataSource={schools}
        columns={columns}
        rowKey="id"
        loading={loading}
        pagination={{
          current: pagination.current,
          pageSize: pagination.pageSize,
          total: pagination.total,
          onChange: onPageChange,
          showSizeChanger: true,
          showTotal: (total) => `Total ${total} schools`,
        }}
      />
    </div>
  );
};
