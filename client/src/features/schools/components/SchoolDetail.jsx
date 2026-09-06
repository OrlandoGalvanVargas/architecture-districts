import {
  Button,
  Card,
  Descriptions,
  Space,
  Tag,
  Popconfirm,
  Statistic,
  Row,
  Col,
  theme,
} from "antd";
import {
  ArrowLeftOutlined,
  DeleteOutlined,
  EditOutlined,
  ApiOutlined,
  TeamOutlined,
  ReadOutlined,
} from "@ant-design/icons";
import { getSchoolLevelLabel, getSchoolTypeLabel } from "../constants/schoolConstants";
import "./SchoolDetail.css";

export const SchoolDetail = ({ school, isDeleting = false, onEdit, onDelete, onBack }) => {
  const { token } = theme.useToken();

  if (!school) return null;

  const stats = [
    { title: "Student Capacity", value: school.studentCapacity || 0, icon: <ReadOutlined /> },
    { title: "Faculty Members", value: school.facultyCount || 0, icon: <TeamOutlined /> },
    { title: "Beacons", value: school.beaconCount || 0, icon: <ApiOutlined /> },
  ];

  return (
    <div>
      {}
      <Row gutter={16} style={{ marginBottom: 16 }}>
        {stats.map((stat) => (
          <Col xs={24} sm={8} key={stat.title}>
            <Card>
              <Space align="start" size={14}>
                <div
                  className="school-stat-icon"
                  style={{ backgroundColor: token.colorPrimaryBg, color: token.colorPrimary }}
                >
                  {stat.icon}
                </div>
                <Statistic title={stat.title} value={stat.value} />
              </Space>
            </Card>
          </Col>
        ))}
      </Row>

      <Card>
        {}
        <div className="school-detail-header">
          <Space wrap size={10}>
            <span className="school-detail-name">{school.name}</span>
            <Tag color="blue">{school.schoolCode}</Tag>
            <Tag color={school.isActive ? "green" : "red"}>
              {school.isActive ? "Active" : "Inactive"}
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
                title="Delete school"
                description={`Are you sure you want to delete "${school.name}"?`}
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
          <Descriptions.Item label="School Name" span={2}>
            {school.name}
          </Descriptions.Item>
          <Descriptions.Item label="Code">{school.schoolCode}</Descriptions.Item>
          <Descriptions.Item label="District">{school.districtName || "N/A"}</Descriptions.Item>

          <Descriptions.Item label="Level">{getSchoolLevelLabel(school.level)}</Descriptions.Item>
          <Descriptions.Item label="Type">{getSchoolTypeLabel(school.type)}</Descriptions.Item>

          <Descriptions.Item label="State">{school.state}</Descriptions.Item>
          <Descriptions.Item label="City">{school.city}</Descriptions.Item>

          <Descriptions.Item label="ZIP Code">{school.zipCode}</Descriptions.Item>
          <Descriptions.Item label="Phone">{school.phone || "N/A"}</Descriptions.Item>

          <Descriptions.Item label="Address" span={2}>
            {school.address}
          </Descriptions.Item>

          <Descriptions.Item label="Contact Email" span={2}>
            {school.contactEmail || "N/A"}
          </Descriptions.Item>

          <Descriptions.Item label="Created At">
            {new Date(school.createdAt).toLocaleDateString("en-US", {
              year: "numeric",
              month: "long",
              day: "numeric",
            })}
          </Descriptions.Item>
          <Descriptions.Item label="Last Updated">
            {school.updatedAt
              ? new Date(school.updatedAt).toLocaleDateString("en-US", {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                })
              : "Never"}
          </Descriptions.Item>
        </Descriptions>
      </Card>
    </div>
  );
};
