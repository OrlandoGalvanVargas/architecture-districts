import { Spin, theme } from "antd";
import "./LoadingSpinner.css";

export const LoadingSpinner = ({
  size = "default",
  description = "Loading...",
  fullScreen = false,
  overlay = false,
}) => {
  const { token } = theme.useToken();

  if (fullScreen) {
    return (
      <div
        className="loading-spinner loading-spinner--fullscreen"
        style={{ backgroundColor: token.colorBgContainer }}
      >
        <Spin size="large" tip={description}>
          <div className="loading-spinner__content" />
        </Spin>
      </div>
    );
  }

  return (
    <div
      className={`loading-spinner ${overlay ? "loading-spinner--overlay" : ""}`}
      style={overlay ? { backgroundColor: token.colorBgMask } : undefined}
    >
      <Spin size={size} tip={description}>
        <div className="loading-spinner__content" />
      </Spin>
    </div>
  );
};
