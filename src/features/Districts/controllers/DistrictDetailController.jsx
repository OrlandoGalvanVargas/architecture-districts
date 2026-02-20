import { useParams, useNavigate } from "react-router-dom";
import { Popconfirm } from "antd";
import { useNofitication } from "../../../contexts/Notification";
import { useDistrict } from "../hooks/useDistrict";
import { useDistrictDelete } from "../hooks/useDistrictDelete";
import { LoadingSpinner } from "../../../components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "../../../components/common/ErrorMessage/ErrorMessage";
import { DistrictDetail } from "../components/DistrictDetail";

export const DistrictDetailController = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const notification = useNofitication();

  const { district, isLoading, error, refetch } = useDistrict(id);
  const { deleteDistrict, isDeleting } = useDistrictDelete({
    onSuccess: () => {
      notification.showSuccess("District deleted successfully");
      navigate("/districts");
    },
    onError: (err) => {
      notification.showError(err.message);
    },
  });

  const handleEdit = () => {
    navigate(`/districts/${id}/edit`);
  };

  const handleDelete = async () => {
    try {
      await deleteDistrict(id);
    } catch (error) {
      console.log(error);
    }
  };

  const handleOnBack = () => {
    navigate("/districts");
  };

  if (isLoading) {
    return <LoadingSpinner description="Loading district details..." />;
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={refetch} />;
  }

  return (
    <Popconfirm
      title="Delete district"
      description="Are you sure you want to delete this district? This action cannot be undone."
      onConfirm={handleDelete}
      okText="Yes, delete"
      cancelText="Cancel"
      okButtonProps={{ danger: true, loading: isDeleting }}
    >
      <div>
        <DistrictDetail
          district={district}
          onDelete={() => {}}
          onEdit={handleEdit}
          onBack={handleOnBack}
        />
      </div>
    </Popconfirm>
  );
};
