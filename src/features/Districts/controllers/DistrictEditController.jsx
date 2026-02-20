import { Card } from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { useNofitication } from "../../../contexts/Notification";
import { useDistrict } from "../hooks/useDistrict";
import { LoadingSpinner } from "../../../components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "../../../components/common/ErrorMessage/ErrorMessage";
import { DistrictForm } from "../components/DistrictForm";
import { useUpdateDistrict } from "../hooks/useUpdateDistrict";

export const DistrictEditController = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const notification = useNofitication();

  const { district, isLoading: isLoadingDistrict, error } = useDistrict(id);

  const { updateDistrict, isUpdating } = useUpdateDistrict({
    onSuccess: () => {
      notification.showSuccess("District updated successfully");
      navigate(`/districts/${id}`);
    },
    onError: (err) => {
      notification.showError(err.message);
    },
  });

  const handleSubmit = async (values) => {
    await updateDistrict(id, values);
  };

  const handleCancel = () => {
    navigate(`/districts/${id}`);
  };

  if (isLoadingDistrict) {
    return <LoadingSpinner description="Loading district...." />;
  }

  if (error) {
    return <ErrorMessage error={error} />;
  }

  return (
    <Card>
      <DistrictForm
        initialValues={district}
        onSubmit={handleSubmit}
        loading={isUpdating}
        onCancel={handleCancel}
      />
    </Card>
  );
};
