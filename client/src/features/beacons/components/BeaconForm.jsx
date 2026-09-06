import { Form, Input, Select, Button, Row, Col, Space, Radio, Divider } from "antd";
import { SaveOutlined, CloseOutlined } from "@ant-design/icons";
import { useState } from "react";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { SchoolSelect } from "@/components/common/SchoolSelect/SchoolSelect";
import { BEACON_TYPES, BEACON_STATUSES } from "../constants/beaconConstants";
import { logger } from "@/services/logger.service";

const { Option } = Select;

export const BeaconForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
  apiErrorMapper = null,
}) => {
  const [form] = Form.useForm();
  const isEditMode = !!initialValues;

  const getInitialAssignment = () => {
    if (!initialValues) return "none";
    if (initialValues.districtId && initialValues.districtId > 0) return "district";
    if (initialValues.schoolId && initialValues.schoolId > 0) return "school";
    return "none";
  };

  const [assignment, setAssignment] = useState(getInitialAssignment());

  const handleAssignmentChange = (e) => {
    setAssignment(e.target.value);
    form.setFieldsValue({ districtId: undefined, schoolId: undefined });
  };

  const handleSubmit = async (values) => {
    try {
      const payload = {
        deviceName: values.deviceName,
        type: values.type,
      };

      if (isEditMode) {
        payload.status = values.status;
      } else {
        payload.serialNumber = values.serialNumber;
      }

      if (assignment === "district") {
        payload.districtId = values.districtId;
      } else if (assignment === "school") {
        payload.schoolId = values.schoolId;
      }

      await onSubmit(payload);
      if (!isEditMode) {
        form.resetFields();
        setAssignment("none");
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
      logger.warn("Beacon form submission failed", error);
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
            label="Device Name"
            name="deviceName"
            rules={[
              { required: true, message: "Please enter device name" },
              { min: 3, message: "Name must be at least 3 characters" },
              { max: 200, message: "Name must not exceed 200 characters" },
            ]}
          >
            <Input placeholder="Beacon device name" />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          {isEditMode ? (
            <Form.Item label="Serial Number" name="serialNumber" tooltip="Read-only after creation">
              <Input disabled />
            </Form.Item>
          ) : (
            <Form.Item
              label="Serial Number"
              name="serialNumber"
              tooltip="Use uppercase letters, numbers, and hyphens only"
              rules={[
                { required: true, message: "Please enter serial number" },
                { max: 100, message: "Serial must not exceed 100 characters" },
                {
                  pattern: /^[A-Z0-9-]+$/,
                  message: "Only uppercase letters, numbers, and hyphens",
                },
              ]}
            >
              <Input
                placeholder="e.g., BCN-001"
                onChange={(e) => {
                  e.target.value = e.target.value.toUpperCase();
                }}
              />
            </Form.Item>
          )}
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Configuration
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="Type"
            name="type"
            rules={[{ required: true, message: "Please select type" }]}
          >
            <Select placeholder="Select beacon type">
              {Object.entries(BEACON_TYPES).map(([value, label]) => (
                <Option key={value} value={Number(value)}>
                  {label}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
        {isEditMode && (
          <Col xs={24} md={12}>
            <Form.Item
              label="Status"
              name="status"
              rules={[{ required: true, message: "Please select status" }]}
            >
              <Select placeholder="Select status">
                {Object.entries(BEACON_STATUSES).map(([value, label]) => (
                  <Option key={value} value={Number(value)}>
                    {label}
                  </Option>
                ))}
              </Select>
            </Form.Item>
          </Col>
        )}
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Assignment
      </Divider>

      <Form.Item label="Assignment" required>
        <Radio.Group value={assignment} onChange={handleAssignmentChange} disabled={loading}>
          <Radio.Button value="none">None</Radio.Button>
          <Radio.Button value="district">District</Radio.Button>
          <Radio.Button value="school">School</Radio.Button>
        </Radio.Group>
      </Form.Item>

      {assignment === "district" && (
        <Form.Item
          label="District"
          name="districtId"
          rules={[{ required: true, message: "Please select district" }]}
        >
          <DistrictSelect />
        </Form.Item>
      )}

      {assignment === "school" && (
        <Form.Item
          label="School"
          name="schoolId"
          rules={[{ required: true, message: "Please select school" }]}
        >
          <SchoolSelect />
        </Form.Item>
      )}

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={loading} icon={<SaveOutlined />}>
            {isEditMode ? "Update Beacon" : "Create Beacon"}
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
