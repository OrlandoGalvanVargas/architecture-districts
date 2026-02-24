import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { DistrictForm } from "../components/DistrictForm";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const DistrictCreateController = withController(
  ({ loading, actions }) => {
    const createDistrict = actions.createDistrict;
    const isCreating = loading.createDistrict;

    const navigate = useAppNavigation();
    const notification = useNotification();

    const handleSubmit = async (values) => {
      try {
        await createDistrict(values);
        notification.showSuccess("District created successfully");
        handleCancel();
      } catch (error) {
        notification.showError(error.message);
      }
    };

    const handleCancel = () => {
      navigate.goToDistricts();
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
