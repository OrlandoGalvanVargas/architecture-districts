import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useNotification } from "@/contexts/Notification";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { DistrictForm } from "../components/DistrictForm";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect } from "react";

export const DistrictEditController = withController(
  ({ data, loading, errors, actions, districtId, setCallbacks }) => {
    const district = data.district;
    const isLoadingDistrict = loading.district;
    const isUpdating = loading.updateDistrict;
    const error = errors.district;
    const updateDistrict = actions.updateDistrict;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("updateDistrict", {
        onSuccess: () => {
          notification.showSuccess("District update successfully");
          navigate.goToDistrictDetail(districtId);
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [districtId, setCallbacks]);

    const handleSubmit = (values) => {
      updateDistrict(districtId, values);
    };

    const handleCancel = () => {
      navigate.goToDistrictDetail(districtId);
    };

    if (isLoadingDistrict) {
      return <LoadingSpinner description="Loading district..." />;
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
  },
  {
    services: {
      district: {
        path: "districts.getById",
        immediate: false,
      },
      updateDistrict: {
        path: "districts.update",
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
