import {
  Form,
  Input,
  Select,
  Button,
  Row,
  Col,
  InputNumber,
  Switch,
  Space,
} from "antd";

const { Option } = Select;

const US_STATES = [
  "AL",
  "AK",
  "AZ",
  "AR",
  "CA",
  "CO",
  "CT",
  "DE",
  "FL",
  "GA",
  "HI",
  "ID",
  "IL",
  "IN",
  "IA",
  "KS",
  "KY",
  "LA",
  "ME",
  "MD",
  "MA",
  "MI",
  "MN",
  "MS",
  "MO",
  "MT",
  "NE",
  "NV",
  "NH",
  "NJ",
  "NM",
  "NY",
  "NC",
  "ND",
  "OH",
  "OK",
  "OR",
  "PA",
  "RI",
  "SC",
  "SD",
  "TN",
  "TX",
  "UT",
  "VT",
  "VA",
  "WA",
  "WV",
  "WI",
  "WY",
];

const SCHOOL_LEVELS = [
  { label: "Elementary", value: 0 },
  { label: "Middle", value: 1 },
  { label: "High", value: 2 },
  { label: "K12", value: 3 },
  { label: "Prek", value: 4 },
];

const SCHOOL_TYPES = [
  { label: "Public", value: 0 },
  { label: "Charter", value: 1 },
  { label: "Magnet", value: 2 },
  { label: "Alternative", value: 3 },
];

export const SchoolForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
}) => {
  const [form] = Form.useForm();

  const handleSubmit = async (values) => {
    try {
      await onSubmit(values);
      if (!initialValues) form.resetFields();
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
        <Col xs={24} md={16}>
          <Form.Item
            label="School Name"
            name="name"
            rules={[{ required: true }]}
          >
            <Input placeholder="Lincoln Elementary School" disabled={loading} />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="School Code"
            name="schoolCode"
            rules={[{ required: true }]}
          >
            <Input placeholder="LNE001" disabled={loading} />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="Level"
            name="level"
            rules={[{ required: true, message: "Please select level" }]}
          >
            <Select placeholder="Select level" disabled={loading}>
              {SCHOOL_LEVELS.map((item) => (
                <Option key={item.value} value={item.value}>
                  {item.label}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            label="Type"
            name="type"
            rules={[{ required: true, message: "Please select type" }]}
          >
            <Select placeholder="Select type" disabled={loading}>
              {SCHOOL_TYPES.map((item) => (
                <Option key={item.value} value={item.value}>
                  {item.label}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} md={8}>
          <Form.Item label="State" name="state" rules={[{ required: true }]}>
            <Select placeholder="Select State" disabled={loading}>
              {US_STATES.map((s) => (
                <Option key={s} value={s}>
                  {s}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item label="City" name="city" rules={[{ required: true }]}>
            <Input placeholder="Select city" disabled={loading} />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="ZIP code"
            name="zipCode"
            rules={[
              { required: true },
              { pattern: /^\d{5}$/ },
              { message: "5 digits required" },
            ]}
          >
            <Input placeholder="90012" disabled={loading} />
          </Form.Item>
        </Col>
      </Row>

      <Form.Item label="Address" name="address" rules={[{ required: true }]}>
        <Input placeholder="123 Main st" disabled={loading} />
      </Form.Item>

      <Row gutter={16}>
        <Col xs={24} md={8}>
          <Form.Item
            label="Student capacity"
            name="studentCapacity"
            rules={[{ required: true }]}
          >
            <InputNumber
              min={1}
              max={10000}
              style={{ width: "100%" }}
              disabled={loading}
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item label="Phone" name="phone">
            <Input placeholder="(555) 000-0000" disabled={loading} />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="Contact email"
            name="contactEmail"
            rules={[{ type: "email", message: "Invalid email" }]}
          >
            <Input placeholder="principal@school.edu" disabled={loading} />
          </Form.Item>
        </Col>
      </Row>

      {initialValues && (
        <Form.Item label="Active" name="isActive" valuePropName="checked">
          <Switch disabled={loading} />
        </Form.Item>
      )}

      <Form.Item
        label="District Id"
        name="districtId"
        rules={[{ required: true }]}
      >
        <InputNumber
          min={1}
          style={{ width: "100%" }}
          disabled={loading || !!initialValues}
        />
      </Form.Item>

      <Row justify="end">
        <Space>
          {onCancel && (
            <Button onClick={onCancel} disabled={loading}>
              Cancel
            </Button>
          )}
          <Button type="primary" htmlType="submit" loading={loading}>
            {initialValues ? "Update school" : "Create school"}
          </Button>
        </Space>
      </Row>
    </Form>
  );
};
