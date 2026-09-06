import { Form, Input, Select, Button, Row, Col, Space, Radio, Switch, Divider } from "antd";
import { SaveOutlined, CloseOutlined } from "@ant-design/icons";
import { useState, useEffect } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { SchoolSelect } from "@/components/common/SchoolSelect/SchoolSelect";
import { BeaconSelect } from "@/components/common/BeaconSelect/BeaconSelect";
import { TITLES, DEPARTMENTS } from "../constants/facultyConstants";
import { USER_ROLES } from "@/features/users/constants/userConstants";
import { logger } from "@/services/logger.service";

const { Option } = Select;

export const FacultyForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
  apiErrorMapper = null,
}) => {
  const [form] = Form.useForm();
  const { user: currentUser } = useAuth();
  const isEditMode = !!initialValues;

  const isSchoolAdmin = currentUser?.role === USER_ROLES.SchoolAdmin;
  const isDistrictAdmin = currentUser?.role === USER_ROLES.DistrictAdmin;

  const getInitialManualAssignment = () => {
    if (initialValues?.districtId) return "district";
    if (initialValues?.schoolId) return "school";
    return "district";
  };

  const [manualAssignment, setManualAssignment] = useState(getInitialManualAssignment());

  const assignment = isSchoolAdmin ? "school" : manualAssignment;

  useEffect(() => {
    if (isSchoolAdmin) {
      form.setFieldsValue({
        schoolId: currentUser.entityId,
        districtId: undefined,
      });
    } else if (isDistrictAdmin && assignment === "district") {
      form.setFieldsValue({ districtId: currentUser.entityId });
    }
  }, [isSchoolAdmin, isDistrictAdmin, assignment, currentUser, form]);

  const handleAssignmentChange = (e) => {
    if (isSchoolAdmin) return;
    setManualAssignment(e.target.value);
    form.setFieldsValue({ districtId: undefined, schoolId: undefined });
  };

  const handleSubmit = async (values) => {
    try {
      const payload = {
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phoneNumber: values.phoneNumber,
        title: values.title,
        department: values.department,
      };

      if (assignment === "district") {
        payload.districtId = values.districtId;
      } else if (assignment === "school") {
        payload.schoolId = values.schoolId;
      }

      if (values.beaconId) {
        payload.beaconId = values.beaconId;
      }

      if (isEditMode) {
        payload.isActive = values.isActive;
      }

      await onSubmit(payload);
      if (!isEditMode) {
        form.resetFields();
        setManualAssignment(getInitialManualAssignment());
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
      logger.warn("Faculty form submission failed", error);
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
        Personal Information
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="First Name"
            name="firstName"
            rules={[
              { required: true, message: "Please enter first name" },
              { min: 2, message: "First name must be at least 2 characters" },
              { max: 100, message: "First name must not exceed 100 characters" },
            ]}
          >
            <Input placeholder="John" />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            label="Last Name"
            name="lastName"
            rules={[
              { required: true, message: "Please enter last name" },
              { min: 2, message: "Last name must be at least 2 characters" },
              { max: 100, message: "Last name must not exceed 100 characters" },
            ]}
          >
            <Input placeholder="Doe" />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="Email"
            name="email"
            rules={[
              { required: true, message: "Please enter email" },
              { type: "email", message: "Please enter a valid email" },
              { max: 256, message: "Email must not exceed 256 characters" },
            ]}
          >
            <Input placeholder="faculty@school.edu" />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            label="Phone Number"
            name="phoneNumber"
            rules={[{ pattern: /^[0-9\-+() ]+$/, message: "Invalid phone number" }]}
          >
            <Input placeholder="(555) 000-0000" />
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Role Details
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="Title"
            name="title"
            rules={[{ required: true, message: "Please select title" }]}
          >
            <Select placeholder="Select title" showSearch optionFilterProp="children">
              {TITLES.map((title) => (
                <Option key={title} value={title}>
                  {title}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            label="Department"
            name="department"
            rules={[{ required: true, message: "Please select department" }]}
          >
            <Select placeholder="Select department" showSearch optionFilterProp="children">
              {DEPARTMENTS.map((dept) => (
                <Option key={dept} value={dept}>
                  {dept}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Assignment
      </Divider>

      <Form.Item label="Assignment" required>
        <Radio.Group
          value={assignment}
          onChange={handleAssignmentChange}
          disabled={loading || isSchoolAdmin}
        >
          <Radio.Button value="district" disabled={isSchoolAdmin}>
            District
          </Radio.Button>
          <Radio.Button value="school">School</Radio.Button>
        </Radio.Group>
      </Form.Item>

      {assignment === "district" && (
        <Form.Item
          label="District"
          name="districtId"
          rules={[{ required: true, message: "Please select district" }]}
        >
          <DistrictSelect disabled={isDistrictAdmin} />
        </Form.Item>
      )}

      {assignment === "school" && (
        <Form.Item
          label="School"
          name="schoolId"
          rules={[{ required: true, message: "Please select school" }]}
        >
          <SchoolSelect
            disabled={isSchoolAdmin}
            districtId={isDistrictAdmin ? currentUser.entityId : undefined}
          />
        </Form.Item>
      )}

      <Form.Item label="Beacon (optional)" name="beaconId">
        <BeaconSelect />
      </Form.Item>

      {isEditMode && (
        <Form.Item label="Active" name="isActive" valuePropName="checked">
          <Switch />
        </Form.Item>
      )}

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={loading} icon={<SaveOutlined />}>
            {isEditMode ? "Update Faculty" : "Create Faculty"}
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
