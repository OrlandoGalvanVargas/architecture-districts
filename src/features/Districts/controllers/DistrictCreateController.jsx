import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { DistrictForm } from "../components/DistrictForm";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect } from "react";

export const DistrictCreateController = withController(
  ({ loading, actions, setCallbacks }) => {
    const createDistrict = actions.createDistrict;
    const isCreating = loading.createDistrict;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("createDistrict", {
        onSuccess: () => {
          notification.showSuccess("District created successfully");
          navigate.goToDistricts();
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [setCallbacks]);

    const handleSubmit = (values) => {
      createDistrict(values);
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
