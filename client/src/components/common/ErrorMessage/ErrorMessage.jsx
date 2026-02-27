import { Alert, Button } from "antd";
import "./ErrorMessage.css";

export const ErrorMessage = ({
  error,
  onRetry = null,
  showDetails = false,
}) => {
  if (!error) return null;

  const getMessage = () => {
    if (typeof error === "string") return error;
    return error.message || "An error occurred";
  };

  return (
    <div className="error-message">
      <Alert
        title="Error"
        description={
          <div>
            <p>{getMessage()}</p>
            {showDetails && error.status && (
              <p className="error-details">Status: {error.status}</p>
            )}
            {onRetry && (
              <Button type="primary" onClick={onRetry} style={{ marginTop: 8 }}>
                Retry
              </Button>
            )}
          </div>
        }
        type="error"
        showIcon
      />
    </div>
  );
};
