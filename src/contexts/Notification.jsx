import { message } from "antd";
import { createContext, useCallback, useContext } from "react";

const NotificationContext = createContext(null);

export const NotificationProvider = ({ children }) => {
  const [messageApi, contextHolder] = message.useMessage();

  const showSuccess = useCallback(
    (content) => {
      messageApi.success(content);
    },
    [messageApi],
  );

  const showError = useCallback(
    (content) => {
      messageApi.error(content);
    },
    [messageApi],
  );

  const showWarning = useCallback(
    (content) => {
      messageApi.warning(content);
    },
    [messageApi],
  );

  const showInfo = useCallback(
    (content) => {
      messageApi.info(content);
    },
    [messageApi],
  );

  const value = {
    showSuccess,
    showError,
    showWarning,
    showInfo,
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
      {contextHolder}
    </NotificationContext.Provider>
  );
};

export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context)
    throw new Error("useNotification must be used within NotificationProvider");

  return context;
};
