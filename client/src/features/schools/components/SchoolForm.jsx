import { Form, Input, Select, Button, Row, Col, InputNumber, Space, Switch, Divider } from "antd";
import { SaveOutlined, CloseOutlined } from "@ant-design/icons";
import { SCHOOL_LEVELS, SCHOOL_TYPES } from "../constants/schoolConstants";
import { US_STATES } from "../constants/schoolConstants";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { logger } from "@/services/logger.service";

const { Option } = Select;

export const SchoolForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
  apiErrorMapper = null,
}) => {
  const [form] = Form.useForm();

  const handleSubmit = async (values) => {
    try {
      await onSubmit(values);
      if (!initialValues) {
        form.resetFields();
      }
    } catch (error) {
      if (apiErrorMapper) {
        const fieldErrors = apiErrorMapper(error);
        if (fieldErrors && Object.keys(fieldErrors).length > 0) {
          form.setFields(
            Object.entries(fieldErrors).map(([name, errors]) => ({
              name,
              errors: [errors],
            }))
          );
        }
      }
      logger.warn("School form submission failed", error);
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
        <Col xs={24} md={16}>
          <Form.Item
            label="School Name"
            name="name"
            rules={[
              { required: true, message: "Please enter school name" },
              { min: 3, message: "Name must be at least 3 characters" },
              { max: 200, message: "Name must not exceed 200 characters" },
            ]}
          >
            <Input placeholder="Lincoln Elementary School" />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="School Code"
            name="schoolCode"
            tooltip="Use uppercase letters, numbers, and hyphens only"
            rules={[
              { required: true, message: "Please enter school code" },
              { max: 50, message: "Code must not exceed 50 characters" },
              {
                pattern: /^[A-Z0-9-]+$/,
                message: "Only uppercase letters, numbers, and hyphens",
              },
            ]}
          >
            <Input
              placeholder="e.g., LNE001"
              onChange={(e) => {
                e.target.value = e.target.value.toUpperCase();
              }}
            />
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
            <Select placeholder="Select level">
              {Object.entries(SCHOOL_LEVELS).map(([value, label]) => (
                <Option key={value} value={Number(value)}>
                  {label}
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
            <Select placeholder="Select type">
              {Object.entries(SCHOOL_TYPES).map(([value, label]) => (
                <Option key={value} value={Number(value)}>
                  {label}
                </Option>
              ))}
            </Select>
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
            <Input placeholder="City" />
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
        Capacity & Contact
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={8}>
          <Form.Item
            label="Student Capacity"
            name="studentCapacity"
            rules={[{ required: true, message: "Please enter capacity" }]}
          >
            <InputNumber min={1} max={10000} style={{ width: "100%" }} placeholder="e.g., 450" />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="Phone"
            name="phone"
            rules={[{ pattern: /^[0-9\-+() ]+$/, message: "Invalid phone number" }]}
          >
            <Input placeholder="(555) 000-0000" />
          </Form.Item>
        </Col>
        <Col xs={24} md={8}>
          <Form.Item
            label="Contact Email"
            name="contactEmail"
            rules={[{ type: "email", message: "Invalid email" }]}
          >
            <Input placeholder="principal@school.edu" />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        District Assignment
      </Divider>

      <Form.Item
        label="District"
        name="districtId"
        rules={[{ required: true, message: "Please select district" }]}
        tooltip={initialValues ? "District cannot be changed after creation" : undefined}
      >
        <DistrictSelect disabled={!!initialValues} />
      </Form.Item>

      {initialValues && (
        <Form.Item label="Active" name="isActive" valuePropName="checked">
          <Switch />
        </Form.Item>
      )}

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={loading} icon={<SaveOutlined />}>
            {initialValues ? "Update School" : "Create School"}
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
