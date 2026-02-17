import { Spin } from "antd";
import "./LoadingSpinner.css";

export const LoadingSpinner = ({
  size = "default",
  description = "Loading...",
}) => {
  return (
    <div className="loading-spinner">
      <Spin size={size} description={description} />
    </div>
  );
};
