import { Form, Input, Button, Card, Alert, Typography } from "antd";
import { UserOutlined, LockOutlined } from "@ant-design/icons";
import { useState } from "react";
import "./LoginForm.css";

const { Title, Text } = Typography;

export const LoginForm = ({ onSubmit }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (values) => {
    setLoading(true);
    setError(null);

    const result = await onSubmit(values);

    if (!result?.success) {
      setError(result?.error || "Login failed. Please try again.");
      setLoading(false);
    }
  };

  return (
    <Card className="login-card">
      <div className="login-header">
        <Title level={3} className="login-title">
          Welcome back
        </Title>
        <Text type="secondary">Sign in to your FacilityOS account</Text>
      </div>

      {error && (
        <Alert
          message="Login failed"
          description={error}
          type="error"
          showIcon
          closable
          onClose={() => setError(null)}
          className="login-error"
        />
      )}

      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{ email: "", password: "" }}
        size="large"
        requiredMark={false}
      >
        <Form.Item
          name="email"
          label="Email"
          rules={[
            { required: true, message: "Please enter your email" },
            { type: "email", message: "Please enter a valid email address" },
          ]}
        >
          <Input
            prefix={<UserOutlined />}
            placeholder="you@school.edu"
            autoComplete="email"
            autoFocus
          />
        </Form.Item>

        <Form.Item
          name="password"
          label="Password"
          rules={[
            { required: true, message: "Please enter your password" },
            { min: 6, message: "Password must be at least 6 characters" },
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder="Enter your password"
            autoComplete="current-password"
          />
        </Form.Item>

        <Form.Item className="login-submit">
          <Button type="primary" htmlType="submit" loading={loading} block>
            Sign In
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};
