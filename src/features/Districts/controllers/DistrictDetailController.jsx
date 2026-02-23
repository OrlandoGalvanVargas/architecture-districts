import { withController } from "@/reactive/withController";
import { useNavigate } from "react-router-dom";
import { useNotification } from "@/contexts/Notification";
import { Popconfirm } from "antd";
import { DistrictDetail } from "../components/DistrictDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";

export const DistrictDetailController = withController(
  ({ data, loading, errors, actions, districtId }) => {
    const navigate = useNavigate();
    const notification = useNotification();

    const district = data.district;
    const isLoading = loading.district;
    const error = errors.district;
    const isDeleting = loading.deleteDistrict;

    const refetchDistrict = actions.district;
    const deleteDistrict = actions.deleteDistrict;

    const handleEdit = () => {
      navigate(`/districts/${districtId}/edit`);
    };

    const handleDelete = async () => {
      try {
        await deleteDistrict(districtId);
        notification.showSuccess("District deleted successfully");
        navigate("/districts");
      } catch (error) {
        notification.showError(error.message);
      }
    };

    const handleBack = () => {
      navigate("/districts");
    };

    if (isLoading) {
      return <LoadingSpinner description="Loading district details..." />;
    }

    if (error) {
      return (
        <ErrorMessage
          error={error}
          onRetry={() => refetchDistrict(districtId)}
        />
      );
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
            onBack={handleBack}
          />
        </div>
      </Popconfirm>
    );
  },
  {
    services: {
      district: {
        path: "districts.getById",
        immediate: false,
      },
      deleteDistrict: {
        path: "districts.delete",
        immediate: false,
      },
    },

    init: ({ actions, props }) => {
      if (props.districtId && actions.district) {
        actions.district(props.districtId);
      }
    },
  },
);
