import {
  Button,
  Card,
  Descriptions,
  Divider,
  Space,
  Tag,
  Popconfirm,
} from "antd";
import {
  ArrowLeftOutlined,
  DeleteOutlined,
  EditOutlined,
} from "@ant-design/icons";

export const DistrictDetail = ({
  district,
  isDeleting,
  onEdit,
  onDelete,
  onBack,
}) => {
  if (!district) return null;

  return (
    <div>
      <Card
        title={
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <span>{district.name}</span>
            <Tag color="blue">{district.code}</Tag>
          </div>
        }
        extra={
          <Space>
            {onBack && (
              <Button
                icon={<ArrowLeftOutlined />}
                onClick={onBack}
                disabled={isDeleting}
              >
                Back
              </Button>
            )}
            {onEdit && (
              <Button
                type="primary"
                icon={<EditOutlined />}
                onClick={onEdit}
                disabled={isDeleting}
              >
                Edit
              </Button>
            )}
            {onDelete && (
              <Popconfirm
                title="Delete district"
                description="Are you sure you want to delete this district? This action cannot be undone."
                onConfirm={onDelete}
                okText="Yes, delete"
                cancelText="Cancel"
                okButtonProps={{ danger: true, loading: isDeleting }}
              >
                <Button danger icon={<DeleteOutlined />} disabled={isDeleting}>
                  Delete
                </Button>
              </Popconfirm>
            )}
          </Space>
        }
      >
        <Descriptions bordered column={2}>
          <Descriptions.Item label="District Name" span={2}>
            {district.name}
          </Descriptions.Item>
          <Descriptions.Item label="District Code">
            {district.code}
          </Descriptions.Item>
          <Descriptions.Item label="School Count">
            <Tag color="green">{district.schoolCount || 0}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="State">{district.state}</Descriptions.Item>
          <Descriptions.Item label="City">{district.city}</Descriptions.Item>
          <Descriptions.Item label="ZIP Code">
            {district.zipCode}
          </Descriptions.Item>
          <Descriptions.Item label="Address" span={2}>
            {district.address}
          </Descriptions.Item>
          {district.description && (
            <Descriptions.Item label="Description" span={2}>
              {district.description}
            </Descriptions.Item>
          )}
        </Descriptions>

        <Divider />

        <Card type="inner" title="Associated Schools" style={{ marginTop: 16 }}>
          <p style={{ color: "#999" }}>
            {district.schoolCount > 0
              ? `This district has ${district.schoolCount} schools. School details will be displayed here.`
              : "No schools assigned to this district yet"}
          </p>
        </Card>
        <Card type="inner" title="Assigned Beacons" style={{ marginTop: 16 }}>
          <p style={{ color: "#999" }}>
            Beacon assignments will be displayed here.
          </p>
        </Card>
      </Card>
    </div>
  );
};
