import { Button, Result } from "antd";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "@/router/routes.config";

export const NotFoundPage = ({
  backPath = ROUTES.HOME,
  backText = "Back to Home",
  subTitle = "Sorry, the page you visited does not exist.",
}) => {
  const navigate = useNavigate();

  return (
    <Result
      status="404"
      title="404"
      subTitle={subTitle}
      extra={
        <Button type="primary" onClick={() => navigate(backPath)}>
          {backText}
        </Button>
      }
    />
  );
};
