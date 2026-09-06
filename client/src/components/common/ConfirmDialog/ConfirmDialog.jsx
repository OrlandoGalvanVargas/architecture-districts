import { Modal, Typography, Space, theme } from "antd";
import { ExclamationCircleOutlined } from "@ant-design/icons";

const { Text } = Typography;

export const ConfirmDialog = ({
  open,
  title = "Confirm Action",
  description = "Are you sure you want to proceed?",
  confirmText = "Confirm",
  cancelText = "Cancel",
  danger = true,
  loading = false,
  onConfirm,
  onCancel,
}) => {
  const { token } = theme.useToken();

  return (
    <Modal
      open={open}
      title={
        <Space>
          <ExclamationCircleOutlined
            style={{ color: danger ? token.colorError : token.colorWarning }}
          />
          {title}
        </Space>
      }
      onOk={onConfirm}
      onCancel={onCancel}
      okText={confirmText}
      cancelText={cancelText}
      okButtonProps={{ danger, loading }}
      destroyOnHidden
    >
      <Text>{description}</Text>
    </Modal>
  );
};
