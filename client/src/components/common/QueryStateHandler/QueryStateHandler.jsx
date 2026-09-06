import { Alert, Button, Space } from "antd";
import { ReloadOutlined } from "@ant-design/icons";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useOnlineStatus } from "@/hooks/useOnlineStatus";

export const QueryStateHandler = ({
  isLoading,
  error,
  data,
  refetch,
  loadingDescription = "Loading...",
  children,
}) => {
  const isOnline = useOnlineStatus();

  if (isLoading && !data) {
    return <LoadingSpinner description={loadingDescription} />;
  }

  if (error && !data) {
    return <ErrorMessage error={error} onRetry={refetch} />;
  }

  const showOfflineWarning = !isOnline || (error && data);

  return (
    <div>
      {showOfflineWarning && (
        <Alert
          type="warning"
          showIcon
          message={
            !isOnline ? "You are offline – showing last loaded data." : "Could not refresh data."
          }
          style={{ marginBottom: 16 }}
          action={
            <Button size="small" icon={<ReloadOutlined />} onClick={refetch} disabled={!isOnline}>
              Retry
            </Button>
          }
        />
      )}
      {children(data)}
    </div>
  );
};
