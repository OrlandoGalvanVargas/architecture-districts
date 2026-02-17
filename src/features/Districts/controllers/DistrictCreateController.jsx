import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { useNofitication } from "@/contexts/Notification";
import { useDistrictCreate } from "../hooks/useDistrictCreate";
import { DistrictForm } from "../components/DistrictForm";

export const DistrictCreateController = () => {
  const navigate = useNavigate();
  const notification = useNofitication();

  const { createDistrict, isCreating } = useDistrictCreate({
    onSuccess: () => {
      notification.showSuccess("District created successfully");
      navigate("/districts");
    },
    onError: (error) => {
      notification.showError(error.message);
    },
  });

  const handleSubmit = async (values) => {
    await createDistrict(values);
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
};
