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
          <span>{school.Name}</span>
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
    ></Card>
  );
};
