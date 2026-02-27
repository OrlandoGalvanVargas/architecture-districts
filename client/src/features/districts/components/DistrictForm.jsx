import { Form, Input, Button, Select, Row, Col, Space } from "antd";

const { TextArea } = Input;

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
      form.resetFields();
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <Form
      form={form}
      layout="vertical"
      initialValues={initialValues}
      onFinish={handleSubmit}
    >
      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="District Name"
            name="name"
            rules={[
              { required: true, message: "Please enter district name" },
              { min: 3, message: "Name must be at least 3 characters" },
            ]}
          >
            <Input placeholder="Enter district name" disabled={loading} />
          </Form.Item>
        </Col>

        <Col xs={24} md={12}>
          <Form.Item
            label="District Code"
            name="code"
            rules={[
              { required: true, message: "Please enter district code" },
              {
                pattern: /^[A-Z0-9-]+$/,
                message: "Only uppercase letters, numbers, and hyphens",
              },
            ]}
          >
            <Input placeholder="e.g., DIST-001" disabled={loading} />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} md={8}>
          <Form.Item
            label="State"
            name="state"
            rules={[{ required: true, message: "Please select state" }]}
          >
            <Select placeholder="Select state" disabled={loading}>
              <Select value="CA">California</Select>
              <Select value="TX">Texas</Select>
              <Select value="NY">New York</Select>
              <Select value="FL">Florida</Select>
            </Select>
          </Form.Item>
        </Col>

        <Col xs={24} md={8}>
          <Form.Item
            label="City"
            name="city"
            rules={[{ required: true, message: "Please enter city" }]}
          >
            <Input placeholder="Enter city" disabled={loading} />
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
            <Input placeholder="12345" disabled={loading} />
          </Form.Item>
        </Col>
      </Row>

      <Form.Item
        label="Address"
        name="address"
        rules={[{ required: true, message: "Please enter address" }]}
      >
        <Input placeholder="Street address" disabled={loading} />
      </Form.Item>

      <Form.Item label="Description" name="description">
        <TextArea
          rows={4}
          placeholder="Optional description"
          disabled={loading}
        />
      </Form.Item>

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={loading}>
            {initialValues ? "Update District" : "Create District"}
          </Button>
          {onCancel && <Button onClick={onCancel}>Cancel</Button>}
        </Space>
      </Form.Item>
    </Form>
  );
};
