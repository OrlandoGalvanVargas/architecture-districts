import { Form, Input, Select, Button, Row, Col, Space, Switch, Divider } from "antd";
import { SaveOutlined, CloseOutlined } from "@ant-design/icons";
import { useState, useEffect } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { SchoolSelect } from "@/components/common/SchoolSelect/SchoolSelect";
import { USER_ROLES } from "../constants/userConstants";
import { logger } from "@/services/logger.service";

const { Option } = Select;

export const UserForm = ({
  initialValues = null,
  onSubmit,
  onCancel = null,
  loading = false,
  apiErrorMapper = null,
}) => {
  const [form] = Form.useForm();
  const { user: currentUser } = useAuth();

  const isEditMode = !!initialValues;

  const allowedRoles = [];
  if (currentUser?.role === USER_ROLES.Admin) {
    allowedRoles.push(USER_ROLES.Admin, USER_ROLES.DistrictAdmin, USER_ROLES.SchoolAdmin);
  } else if (currentUser?.role === USER_ROLES.DistrictAdmin) {
    allowedRoles.push(USER_ROLES.DistrictAdmin, USER_ROLES.SchoolAdmin);
  } else if (currentUser?.role === USER_ROLES.SchoolAdmin) {
    allowedRoles.push(USER_ROLES.SchoolAdmin);
  }

  const isRoleLocked = currentUser?.role === USER_ROLES.SchoolAdmin;

  const [manualRole, setManualRole] = useState(initialValues?.role || undefined);
  const selectedRole = isRoleLocked ? USER_ROLES.SchoolAdmin : manualRole;

  const isEntityLocked =
    currentUser?.role === USER_ROLES.SchoolAdmin ||
    (currentUser?.role === USER_ROLES.DistrictAdmin && selectedRole === USER_ROLES.DistrictAdmin);

  useEffect(() => {
    if (isRoleLocked) {
      form.setFieldsValue({
        role: USER_ROLES.SchoolAdmin,
        entityId: currentUser.entityId,
      });
    } else if (
      currentUser?.role === USER_ROLES.DistrictAdmin &&
      selectedRole === USER_ROLES.DistrictAdmin
    ) {
      form.setFieldsValue({ entityId: currentUser.entityId });
    }
  }, [currentUser, isRoleLocked, selectedRole, form]);

  const handleRoleChange = (value) => {
    setManualRole(value);
    form.setFieldsValue({ entityId: undefined });
    if (currentUser?.role === USER_ROLES.DistrictAdmin && value === USER_ROLES.DistrictAdmin) {
      form.setFieldsValue({ entityId: currentUser.entityId });
    }
  };

  const handleSubmit = async (values) => {
    try {
      const role = values.role;
      let entityType;
      let entityId = values.entityId;
      if (role === USER_ROLES.Admin) {
        entityType = 0;
        entityId = 0;
      } else if (role === USER_ROLES.DistrictAdmin) {
        entityType = 1;
      } else {
        entityType = 2;
      }

      const payload = {
        ...values,
        entityType,
        entityId,
      };

      if (isEditMode) {
        if (payload.password) {
          payload.newPassword = payload.password;
        }
        delete payload.password;
      }

      await onSubmit(payload);
      if (!isEditMode) {
        form.resetFields();
        setManualRole(undefined);
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
      logger.warn("User form submission failed", error);
    }
  };

  const showDistrictSelect = selectedRole === USER_ROLES.DistrictAdmin;
  const showSchoolSelect = selectedRole === USER_ROLES.SchoolAdmin;

  return (
    <Form
      form={form}
      layout="vertical"
      initialValues={initialValues}
      onFinish={handleSubmit}
      disabled={loading}
    >
      <Divider orientation="left" orientationMargin={0} style={{ marginTop: 0 }}>
        Account Information
      </Divider>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label="Full Name"
            name="name"
            rules={[
              { required: true, message: "Please enter full name" },
              { min: 3, message: "Name must be at least 3 characters" },
              { max: 200, message: "Name must not exceed 200 characters" },
            ]}
          >
            <Input placeholder="John Doe" />
          </Form.Item>
        </Col>
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
            <Input placeholder="user@example.com" />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} md={12}>
          <Form.Item
            label={isEditMode ? "New Password" : "Password"}
            name={isEditMode ? "newPassword" : "password"}
            rules={
              isEditMode
                ? [{ min: 6, message: "Password must be at least 6 characters" }]
                : [
                    { required: true, message: "Please enter password" },
                    { min: 6, message: "Password must be at least 6 characters" },
                  ]
            }
            extra={isEditMode ? "Leave blank to keep current password" : "Minimum 6 characters"}
          >
            <Input.Password
              placeholder={isEditMode ? "New password (optional)" : "Enter password"}
              autoComplete="new-password"
            />
          </Form.Item>
        </Col>
        <Col xs={24} md={12}>
          <Form.Item
            label="Role"
            name="role"
            rules={[{ required: true, message: "Please select role" }]}
          >
            <Select
              placeholder="Select role"
              onChange={handleRoleChange}
              value={selectedRole}
              disabled={isRoleLocked}
            >
              {allowedRoles.map((role) => (
                <Option key={role} value={role}>
                  {role}
                </Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Divider orientation="left" orientationMargin={0}>
        Access Scope
      </Divider>

      {showDistrictSelect && (
        <Form.Item
          label="District"
          name="entityId"
          rules={[{ required: true, message: "Please select district" }]}
        >
          <DistrictSelect disabled={isEntityLocked} />
        </Form.Item>
      )}

      {showSchoolSelect && (
        <Form.Item
          label="School"
          name="entityId"
          rules={[{ required: true, message: "Please select school" }]}
        >
          <SchoolSelect
            disabled={isEntityLocked}
            districtId={
              currentUser?.role === USER_ROLES.DistrictAdmin ? currentUser.entityId : undefined
            }
          />
        </Form.Item>
      )}

      {selectedRole === USER_ROLES.Admin && (
        <Form.Item label="Entity" name="entityId" hidden>
          <Input value={0} />
        </Form.Item>
      )}

      {isEditMode && (
        <Form.Item label="Active" name="isActive" valuePropName="checked">
          <Switch />
        </Form.Item>
      )}

      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit" loading={loading} icon={<SaveOutlined />}>
            {isEditMode ? "Update User" : "Create User"}
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
