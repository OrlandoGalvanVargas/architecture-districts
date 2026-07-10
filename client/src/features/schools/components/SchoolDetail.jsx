import { Card, Descriptions, Tag, Button, Space, Popconfirm } from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  ArrowLeftOutlined,
} from "@ant-design/icons";

export const SchoolDetail = ({
  school,
  loading,
  isDeleting,
  onEdit,
  onDelete,
  onBack,
}) => {
  if (!school) return null;

  return (
    <Card
      loading={loading}
      title={
        <Space>
          <span>{school.name}</span>
          <Tag color="blue">{school.schoolCode}</Tag>
          <Tag color={school.isActive ? "green" : "red"}>
            {school.isActive ? "Active" : "Inactive"}
          </Tag>
        </Space>
      }
      extra={
        <Space>
          <Button icon={<ArrowLeftOutlined />} onClick={onBack}>
            Back
          </Button>
          <Button type="primary" icon={<EditOutlined />} onClick={onEdit}>
            Edit
          </Button>
          <Popconfirm
            title="Delete School"
            description="Are you sure you want to delete this school?"
            onConfirm={onDelete}
            okText="Yes, delete"
            cancelText="Cancel"
            okButtonProps={{ danger: true, loading: isDeleting }}
          >
            <Button danger icon={<DeleteOutlined />}>
              Delete
            </Button>
          </Popconfirm>
        </Space>
      }
    >
      <Descriptions bordered column={2}>
        <Descriptions.Item label="Name" span={2}>
          {school.name}
        </Descriptions.Item>
        <Descriptions.Item label="Code">{school.schoolCode}</Descriptions.Item>
        <Descriptions.Item label="District">
          {school.districtName}
        </Descriptions.Item>
        <Descriptions.Item label="Level">{school.level}</Descriptions.Item>
        <Descriptions.Item label="Type">{school.type}</Descriptions.Item>
        <Descriptions.Item label="State">{school.state}</Descriptions.Item>
        <Descriptions.Item lable="City">{school.city}</Descriptions.Item>
        <Descriptions.Item label="ZIP">{school.zipCode}</Descriptions.Item>
        <Descriptions.Item label="Capacity">
          {school.studentCapacity}
        </Descriptions.Item>
        {school.phone && (
          <Descriptions.Item label="Phone">{school.phone}</Descriptions.Item>
        )}
        {school.contactEmail && (
          <Descriptions.Item label="Contact Email">
            {school.contactEmail}
          </Descriptions.Item>
        )}
      </Descriptions>
    </Card>
  );
};
