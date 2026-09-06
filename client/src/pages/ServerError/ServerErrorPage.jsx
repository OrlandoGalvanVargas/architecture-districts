import { Button, Result } from "antd";

export const ServerErrorPage = ({ onRetry = null }) => {
  return (
    <Result
      status="500"
      title="500"
      subTitle="Sorry, something went wrong on our end."
      extra={[
        <Button type="primary" key="retry" onClick={onRetry || (() => window.location.reload())}>
          Try Again
        </Button>,
        <Button key="home" onClick={() => (window.location.href = "/")}>
          Back to Home
        </Button>,
      ]}
    />
  );
};
