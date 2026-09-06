import { Table, Button, Space, Tag, Tooltip, Popconfirm } from "antd";
import { EditOutlined, DeleteOutlined, EyeOutlined } from "@ant-design/icons";
import { getSchoolLevelLabel, getSchoolTypeLabel } from "../constants/schoolConstants";

export const SchoolTable = ({
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
      sorter: true,
      render: (name) => <strong>{name}</strong>,
    },
    {
      title: "Code",
      dataIndex: "schoolCode",
      key: "schoolCode",
      width: 110,
      render: (code) => <Tag color="blue">{code}</Tag>,
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
      title: "Level",
      dataIndex: "level",
      key: "level",
      width: 120,
      render: (level) => getSchoolLevelLabel(level),
    },
    {
      title: "Type",
      dataIndex: "type",
      key: "type",
      width: 120,
      render: (type) => getSchoolTypeLabel(type),
    },
    {
      title: "District",
      dataIndex: "districtName",
      key: "districtName",
      width: 150,
    },
    {
      title: "City",
      dataIndex: "city",
      key: "city",
      width: 120,
    },
    {
      title: "State",
      dataIndex: "state",
      key: "state",
      width: 80,
      align: "center",
    },
    {
      title: "Capacity",
      dataIndex: "studentCapacity",
      key: "studentCapacity",
      width: 100,
      align: "right",
    },
    {
      title: "Beacons",
      dataIndex: "beaconCount",
      key: "beaconCount",
      width: 90,
      align: "center",
    },
    {
      title: "Faculty",
      dataIndex: "facultyCount",
      key: "facultyCount",
      width: 90,
      align: "center",
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
            <Tooltip title="Edit school">
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
              title="Delete school"
              description={`Are you sure you want to delete "${record.name}"?`}
              onConfirm={() => onDelete(record)}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true }}
            >
              <Tooltip title="Delete school">
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
      scroll={{ x: 1300 }}
      pagination={{
        current: pagination.page || 1,
        pageSize: pagination.pageSize || 10,
        total: pagination.totalCount || 0,
        showSizeChanger: true,
        showTotal: (total) => `Total ${total} schools`,
        onChange: (page, pageSize) => {
          onPageChange?.(page, pageSize);
        },
      }}
    />
  );
};
