import { Alert, Button, Space, theme } from "antd";
import { useState } from "react";
import "./ErrorMessage.css";

export const ErrorMessage = ({ error, onRetry = null, showDetails = false, compact = false }) => {
  const [showTechnicalDetails, setShowTechnicalDetails] = useState(false);
  const { token } = theme.useToken();

  if (!error) return null;

  const getMessage = () => {
    if (typeof error === "string") return error;
    return error.friendlyMessage || error.message || "An error occurred";
  };

  const getStatus = () => {
    if (typeof error === "object" && error.status) {
      return error.status;
    }
    return null;
  };

  return (
    <div className={`error-message ${compact ? "error-message--compact" : ""}`}>
      <Alert
        type="error"
        showIcon
        message={getStatus() ? `Error ${getStatus()}` : "Error"}
        description={
          <div>
            <p className="error-message__text">{getMessage()}</p>

            {showDetails && error.details && (
              <Button
                type="link"
                size="small"
                onClick={() => setShowTechnicalDetails(!showTechnicalDetails)}
              >
                {showTechnicalDetails ? "Hide Details" : "Show Details"}
              </Button>
            )}

            {showTechnicalDetails && (
              <pre
                className="error-message__details"
                style={{
                  backgroundColor: token.colorFillTertiary,
                  color: token.colorText,
                  borderRadius: token.borderRadius,
                }}
              >
                {JSON.stringify(error.details, null, 2)}
              </pre>
            )}

            {onRetry && (
              <Space style={{ marginTop: 12 }}>
                <Button type="primary" size="small" onClick={onRetry}>
                  Retry
                </Button>
              </Space>
            )}
          </div>
        }
      />
    </div>
  );
};
