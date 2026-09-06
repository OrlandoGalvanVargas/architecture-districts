import { Button, Card, Descriptions, Space, Tag, Popconfirm } from "antd";
import { ArrowLeftOutlined, DeleteOutlined, EditOutlined } from "@ant-design/icons";
import { getBeaconTypeLabel } from "@/features/beacons/constants/beaconConstants";
import "./FacultyDetail.css";

export const FacultyDetail = ({ faculty, isDeleting = false, onEdit, onDelete, onBack }) => {
  if (!faculty) return null;

  return (
    <Card>
      <div className="faculty-detail-header">
        <Space wrap size={10}>
          <span className="faculty-detail-name">{faculty.fullName}</span>
          <Tag color={faculty.isActive ? "green" : "red"}>
            {faculty.isActive ? "Active" : "Inactive"}
          </Tag>
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
              title="Delete faculty"
              description={`Are you sure you want to delete "${faculty.fullName}"?`}
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
        <Descriptions.Item label="Full Name" span={2}>
          {faculty.fullName}
        </Descriptions.Item>
        <Descriptions.Item label="First Name">{faculty.firstName}</Descriptions.Item>
        <Descriptions.Item label="Last Name">{faculty.lastName}</Descriptions.Item>
        <Descriptions.Item label="Email" span={2}>
          {faculty.email}
        </Descriptions.Item>
        <Descriptions.Item label="Phone">{faculty.phoneNumber || "—"}</Descriptions.Item>
        <Descriptions.Item label="Title">{faculty.title}</Descriptions.Item>
        <Descriptions.Item label="Department">{faculty.department}</Descriptions.Item>
        <Descriptions.Item label="District">{faculty.districtName || "—"}</Descriptions.Item>
        <Descriptions.Item label="School">{faculty.schoolName || "—"}</Descriptions.Item>
        <Descriptions.Item label="Beacon" span={2}>
          {faculty.beaconId ? (
            <Space>
              <Tag color="blue">{faculty.beaconDeviceName || "Assigned"}</Tag>
              {faculty.beaconSerialNumber && <Tag>{faculty.beaconSerialNumber}</Tag>}
              {faculty.beaconType && (
                <Tag color="geekblue">{getBeaconTypeLabel(faculty.beaconType)}</Tag>
              )}
            </Space>
          ) : (
            "—"
          )}
        </Descriptions.Item>
        <Descriptions.Item label="Created At">
          {new Date(faculty.createdAt).toLocaleDateString("en-US", {
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </Descriptions.Item>
        <Descriptions.Item label="Last Updated">
          {faculty.updatedAt
            ? new Date(faculty.updatedAt).toLocaleDateString("en-US", {
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
