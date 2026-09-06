import { Button, Result } from "antd";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "@/router/routes.config";

export const ForbiddenPage = ({
  backPath = ROUTES.HOME,
  backText = "Back to Home",
  subTitle = "Sorry, you are not authorized to access this page.",
}) => {
  const navigate = useNavigate();

  return (
    <Result
      status="403"
      title="403"
      subTitle={subTitle}
      extra={
        <Button type="primary" onClick={() => navigate(backPath)}>
          {backText}
        </Button>
      }
    />
  );
};
