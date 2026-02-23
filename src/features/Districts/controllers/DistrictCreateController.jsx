import { withController } from "@/reactive/withController";
import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { DistrictForm } from "../components/DistrictForm";
import { useNotification } from "@/contexts/Notification";

export const DistrictCreateController = withController(
  ({ loading, actions }) => {
    const navigate = useNavigate();
    const notification = useNotification();

    const createDistrict = actions.createDistrict;
    const isCreating = loading.createDistrict;

    const handleSubmit = async (values) => {
      try {
        await createDistrict(values);
        notification.showSuccess("District created successfully");
        navigate("/districts");
      } catch (error) {
        notification.showError(error.message);
      }
    };

    const handleCancel = () => {
      navigate("/districts");
    };

    return (
      <Card>
        <DistrictForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={isCreating}
        />
      </Card>
    );
  },
  {
    services: {
      createDistrict: {
        path: "districts.create",
        immediate: false,
      },
    },
  },
);
