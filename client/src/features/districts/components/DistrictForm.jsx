import { Form, Input, Button, Select, Row, Col, Space, Divider } from "antd";
import { SaveOutlined, CloseOutlined } from "@ant-design/icons";
import { logger } from "@/services/logger.service";

const { TextArea } = Input;

const US_STATES = [
  { value: "CA", label: "California" },
  { value: "TX", label: "Texas" },
  { value: "NY", label: "New York" },
  { value: "FL", label: "Florida" },
  { value: "AZ", label: "Arizona" },
  { value: "NV", label: "Nevada" },
  { value: "WA", label: "Washington" },
  { value: "OR", label: "Oregon" },
  { value: "CO", label: "Colorado" },
  { value: "IL", label: "Illinois" },
];

export const DistrictForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
}) => {
  const [form] = Form.useForm();

  const handleSubmit = async (values) => {
    try {
      await onSubmit(values);
      if (!initialValues) {
        form.resetFields();
      }
    } catch (error) {
      logger.warn("District form submission failed", error);
    }
  };

  return (
    <Form
      form={form}
      layout="vertical"
      initialValues={initialValues}
      onFinish={handleSubmit}
      disabled={loading}
    >
      <Divider orientation="left" orientationMargin={0} style={{ marginTop: 0 }}>
        Basic Information
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="District Name"
            name="name"
            rules={[
              { required: true, message: "Please enter district name" },
              { min: 3, message: "Name must be at least 3 characters" },
              { max: 200, message: "Name must not exceed 200 characters" },
            ]}
          >
            <Input placeholder="Enter district name" />
          </Form.Item>
        </Col>

        <Col xs={24} md={12}>
          <Form.Item
            label="District Code"
            name="code"
            tooltip="Use uppercase letters, numbers, and hyphens only"
            rules={[
              { required: true, message: "Please enter district code" },
              { max: 50, message: "Code must not exceed 50 characters" },
              {
                pattern: /^[A-Z0-9-]+$/,
                message: "Only uppercase letters, numbers, and hyphens",
              },
            ]}
          >
            <Input
              placeholder="e.g., DIST-001"
              onChange={(e) => {
                e.target.value = e.target.value.toUpperCase();
              }}
            />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Location
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={8}>
          <Form.Item
            label="State"
            name="state"
            rules={[{ required: true, message: "Please select state" }]}
          >
            <Select
              placeholder="Select state"
              showSearch
              optionFilterProp="label"
              options={US_STATES}
            />
          </Form.Item>
        </Col>

        <Col xs={24} md={8}>
          <Form.Item
            label="City"
            name="city"
            rules={[
              { required: true, message: "Please enter city" },
              { max: 100, message: "City must not exceed 100 characters" },
            ]}
          >
            <Input placeholder="Enter city" />
          </Form.Item>
        </Col>

        <Col xs={24} md={8}>
          <Form.Item
            label="ZIP Code"
            name="zipCode"
            rules={[
              { required: true, message: "Please enter ZIP code" },
              { pattern: /^\d{5}$/, message: "ZIP must be 5 digits" },
            ]}
          >
            <Input placeholder="12345" maxLength={5} />
          </Form.Item>
        </Col>
      </Row>

      <Form.Item
        label="Address"
        name="address"
        rules={[
          { required: true, message: "Please enter address" },
          { max: 500, message: "Address must not exceed 500 characters" },
        ]}
      >
        <Input placeholder="Street address" />
      </Form.Item>

      <Divider orientation="left" orientationMargin={0}>
        Additional Details
      </Divider>

      <Form.Item
        label="Description"
        name="description"
        rules={[{ max: 1000, message: "Description must not exceed 1000 characters" }]}
      >
        <TextArea rows={4} placeholder="Optional description" showCount maxLength={1000} />
      </Form.Item>

      <Form.Item className="form-actions">
        <Space>
          <Button type="primary" htmlType="submit" loading={loading} icon={<SaveOutlined />}>
            {initialValues ? "Update District" : "Create District"}
          </Button>
          {onCancel && (
            <Button onClick={onCancel} icon={<CloseOutlined />} disabled={loading}>
              Cancel
            </Button>
          )}
        </Space>
      </Form.Item>
    </Form>
  );
};
