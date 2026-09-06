/* eslint-disable react-refresh/only-export-components */
import { notification, message } from "antd";
import { createContext, useCallback, useContext, useEffect, useMemo } from "react";

const NotificationContext = createContext(null);

export const NotificationProvider = ({ children }) => {
  const [messageApi, messageContextHolder] = message.useMessage();
  const [notificationApi, notificationContextHolder] = notification.useNotification();

  useEffect(() => {
    notification.config({ placement: "topRight", duration: 4.5 });
  }, []);

  const showSuccess = useCallback(
    (content, duration = 3) => {
      messageApi.success({ content, duration });
    },
    [messageApi]
  );

  const showError = useCallback(
    (content, duration = 5) => {
      messageApi.error({ content, duration });
    },
    [messageApi]
  );

  const showWarning = useCallback(
    (content, duration = 4) => {
      messageApi.warning({ content, duration });
    },
    [messageApi]
  );

  const showInfo = useCallback(
    (content, duration = 3) => {
      messageApi.info({ content, duration });
    },
    [messageApi]
  );

  const notifySuccess = useCallback(
    (title, description) => {
      notificationApi.success({ message: title, description });
    },
    [notificationApi]
  );

  const notifyError = useCallback(
    (title, description) => {
      notificationApi.error({ message: title, description });
    },
    [notificationApi]
  );

  const handleApiError = useCallback(
    (error, fallbackMessage = "An error occurred") => {
      const errorText = error?.friendlyMessage || error?.message || fallbackMessage;
      showError(errorText);
      return errorText;
    },
    [showError]
  );

  const value = useMemo(
    () => ({
      showSuccess,
      showError,
      showWarning,
      showInfo,
      notifySuccess,
      notifyError,
      handleApiError,
    }),
    [showSuccess, showError, showWarning, showInfo, notifySuccess, notifyError, handleApiError]
  );

  return (
    <NotificationContext.Provider value={value}>
      {children}
      {messageContextHolder}
      {notificationContextHolder}
    </NotificationContext.Provider>
  );
};

export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error("useNotification must be used within NotificationProvider");
  }
  return context;
};
