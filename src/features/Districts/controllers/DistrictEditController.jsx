import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useNavigate } from "react-router-dom";
import { useNotification } from "@/contexts/Notification";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { DistrictForm } from "../components/DistrictForm";

export const DistrictEditController = withController(
  ({ data, loading, errors, actions, districtId }) => {
    const navigate = useNavigate();
    const notification = useNotification();

    const district = data.district;
    const isLoadingDistrict = loading.district;
    const error = errors.district;
    const isUpdating = loading.updateDistrict;

    const updateDistrict = actions.updateDistrict;

    const handleSubmit = async (values) => {
      try {
        await updateDistrict(districtId, values);
        notification.showSuccess("District updated successfully");
        navigate(`/districts/${districtId}`);
      } catch (error) {
        notification.showError(error.message);
      }
    };

    const handleCancel = () => {
      navigate(`/districts/${districtId}`);
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
