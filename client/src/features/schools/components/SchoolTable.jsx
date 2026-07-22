import { Table, Button, Tag, Space, Select, Switch } from "antd";
import { PlusOutlined, EyeOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

const { Option } = Select;

export const SchoolTable = ({
  schools,
  loading,
  pagination,
  onPageChange,
  onView,
  onCreate,
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
        <Button
          icon={<EyeOutlined />}
          onClick={() => onView(record.id)}
          size="small"
        >
          View
        </Button>
      ),
    },
  ];

  return (
    <div>
      <PageHeader
        title="Schools"
        extra={
          <Button type="primary" icon={<PlusOutlined />} onClick={onCreate}>
            New School
          </Button>
        }
      />
      <Space style={{ marginBottom: 16 }}>
        <Select
          placeholder="Level"
          allowClear
          style={{ width: 140 }}
          onChange={(val) => onFilterChange({ level: val })}
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
          style={{ width: 140 }}
          onChange={(val) => onFilterChange({ type: val })}
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
          defaultChecked
          onChange={(checked) =>
            onFilterChange({ isActive: checked || undefined })
          }
        />
      </Space>
      <Table
        dataSource={schools}
        columns={columns}
        rowKey="id"
        loading={loading}
        pagination={{
          current: pagination.page,
          pageSize: pagination.pageSize,
          total: pagination.total,
          onChange: onPageChange,
        }}
      />
    </div>
  );
};
