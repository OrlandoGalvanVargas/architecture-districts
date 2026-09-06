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
  BankOutlined,
  ApiOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import "./DistrictDetail.css";

export const DistrictDetail = ({ district, isDeleting = false, onEdit, onDelete, onBack }) => {
  const { token } = theme.useToken();

  if (!district) return null;

  const stats = [
    { title: "Schools", value: district.schoolCount || 0, icon: <BankOutlined /> },
    { title: "Beacons", value: district.beaconCount || 0, icon: <ApiOutlined /> },
    { title: "Faculty Members", value: district.facultyCount || 0, icon: <TeamOutlined /> },
  ];

  return (
    <div>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        {stats.map((stat) => (
          <Col xs={24} sm={8} key={stat.title}>
            <Card>
              <Space align="start" size={14}>
                <div
                  className="district-stat-icon"
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
        <div className="district-detail-header">
          <Space wrap size={10}>
            <span className="district-detail-name">{district.name}</span>
            <Tag color="blue">{district.code}</Tag>
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
                title="Delete district"
                description={`Are you sure you want to delete "${district.name}"?`}
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
          <Descriptions.Item label="District Name" span={2}>
            {district.name}
          </Descriptions.Item>
          <Descriptions.Item label="District Code">{district.code}</Descriptions.Item>
          <Descriptions.Item label="State">{district.state}</Descriptions.Item>
          <Descriptions.Item label="City">{district.city}</Descriptions.Item>
          <Descriptions.Item label="ZIP Code">{district.zipCode}</Descriptions.Item>
          <Descriptions.Item label="Address" span={2}>
            {district.address}
          </Descriptions.Item>
          {district.description && (
            <Descriptions.Item label="Description" span={2}>
              {district.description}
            </Descriptions.Item>
          )}
          <Descriptions.Item label="Created At">
            {new Date(district.createdAt).toLocaleDateString("en-US", {
              year: "numeric",
              month: "long",
              day: "numeric",
            })}
          </Descriptions.Item>
          <Descriptions.Item label="Last Updated">
            {district.updatedAt
              ? new Date(district.updatedAt).toLocaleDateString("en-US", {
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
