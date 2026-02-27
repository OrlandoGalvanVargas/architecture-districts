import { withController } from "@/reactive/withController";
import { useNotification } from "@/contexts/Notification";
import { DistrictDetail } from "../components/DistrictDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect } from "react";
import { ErrorMessage } from "../../../components/common/ErrorMessage/ErrorMessage";

export const DistrictDetailController = withController(
  ({ data, loading, errors, actions, districtId, setCallbacks }) => {
    const district = data.district;
    const isLoading = loading.district;
    const isDeleting = loading.deleteDistrict;
    const error = errors.district;
    const refetchDistrict = actions.district;
    const deleteDistrict = actions.deleteDistrict;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("deleteDistrict", {
        onSuccess: () => {
          notification.showSuccess("District deleted successfully");
          navigate.goToDistricts();
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [setCallbacks]);

    const handleEdit = () => {
      navigate.goToDistrictEdit(districtId);
    };

    const handleDelete = () => {
      deleteDistrict(districtId);
    };

    const handleBack = () => {
      navigate.goToDistricts();
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
      <DistrictDetail
        district={district}
        isDeleting={isDeleting}
        onDelete={handleDelete}
        onEdit={handleEdit}
        onBack={handleBack}
      />
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
