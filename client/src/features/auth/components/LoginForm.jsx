import { Form, Input, Button, Card, Alert } from "antd";
import { UserOutlined, LockOutlined } from "@ant-design/icons";

export const LoginForm = ({ onSubmit, loading = false, error = null }) => {
  const [form] = Form.useForm();

  const handleSubmit = (values) => {
    onSubmit(values);
  };

  return (
    <Card
      title="Login to LiveFree Schools"
      style={{ maxWidth: 400, margin: "0 auto" }}
    >
      {error && (
        <Alert
          message="Login Failed"
          description={error}
          type="error"
          showIcon
          style={{ marginBottom: 16 }}
        />
      )}

      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{ email: "", password: "" }}
      >
        <Form.Item
          name="email"
          rules={[
            { required: true, message: "Please enter your email" },
            { type: "email", message: "Please enter a valid email" },
          ]}
        >
          <Input prefix={<UserOutlined />} placeholder="Email" size="large" />
        </Form.Item>

        <Form.Item
          name="password"
          rules={[
            { required: true, message: "Please enter your password" },
            { min: 6, message: "Password must be at least 6 characters" },
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder="Password"
            size="large"
          />
        </Form.Item>

        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            loading={loading}
            size="large"
            block
          >
            Log In
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};
