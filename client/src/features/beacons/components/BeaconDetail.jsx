import { Button, Card, Descriptions, Space, Tag, Popconfirm } from "antd";
import { ArrowLeftOutlined, DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { getBeaconTypeLabel, getBeaconStatusLabel } from "../constants/beaconConstants";
import "./BeaconDetail.css";

const getStatusColor = (status) =>
  status === 1 ? "green" : status === 2 ? "blue" : status === 3 ? "orange" : "red";

export const BeaconDetail = ({ beacon, isDeleting = false, onEdit, onDelete, onBack }) => {
  if (!beacon) return null;

  return (
    <Card>
      <div className="beacon-detail-header">
        <Space wrap size={10}>
          <span className="beacon-detail-name">{beacon.deviceName}</span>
          <Tag color="blue">{beacon.serialNumber}</Tag>
          <Tag color={getStatusColor(beacon.status)}>{getBeaconStatusLabel(beacon.status)}</Tag>
        </Space>

        <Space wrap>
          {onBack && (
            <Button icon={<ArrowLeftOutlined />} onClick={onBack} disabled={isDeleting}>
              Back
            </Button>
          )}
          {onEdit && (
            <Button type="primary" icon={<EditOutlined />} onClick={onEdit} disabled={isDeleting}>
              Edit
            </Button>
          )}
          {onDelete && (
            <Popconfirm
              title="Delete beacon"
              description={`Are you sure you want to delete "${beacon.deviceName}"?`}
              onConfirm={onDelete}
              okText="Yes, Delete"
              cancelText="Cancel"
              okButtonProps={{ danger: true, loading: isDeleting }}
            >
              <Button danger icon={<DeleteOutlined />} disabled={isDeleting}>
                Delete
              </Button>
            </Popconfirm>
          )}
        </Space>
      </div>

      <Descriptions bordered column={{ xs: 1, sm: 1, md: 2 }} size="middle">
        <Descriptions.Item label="Device Name" span={2}>
          {beacon.deviceName}
        </Descriptions.Item>
        <Descriptions.Item label="Serial Number">{beacon.serialNumber}</Descriptions.Item>
        <Descriptions.Item label="Type">{getBeaconTypeLabel(beacon.type)}</Descriptions.Item>
        <Descriptions.Item label="Assigned">
          {beacon.isAssigned ? <Tag color="green">Yes</Tag> : <Tag color="red">No</Tag>}
        </Descriptions.Item>
        <Descriptions.Item label="District">{beacon.districtName || "—"}</Descriptions.Item>
        <Descriptions.Item label="School">{beacon.schoolName || "—"}</Descriptions.Item>
        <Descriptions.Item label="Faculty">{beacon.facultyName || "—"}</Descriptions.Item>
        <Descriptions.Item label="Created At">
          {new Date(beacon.createdAt).toLocaleDateString("en-US", {
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </Descriptions.Item>
        <Descriptions.Item label="Last Updated">
          {beacon.updatedAt
            ? new Date(beacon.updatedAt).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })
            : "Never"}
        </Descriptions.Item>
      </Descriptions>
    </Card>
  );
};
