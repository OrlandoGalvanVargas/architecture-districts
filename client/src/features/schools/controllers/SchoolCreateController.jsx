import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useEffect } from "react";
import { SchoolForm } from "../components/SchoolForm";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const SchoolCreateController = withController(
  ({ loading, actions, setCallbacks }) => {
    const createSchool = actions.createSchool;
    const isCreating = loading.createSchool;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("createSchool", {
        onSuccess: () => {
          notification.showSuccess("School created successfully");
          navigate.goToSchools();
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [setCallbacks]);

    const handleSubmit = (values) => {
      createSchool(values);
    };

    const handleCancel = () => {
      navigate.goToSchools();
    };

    return (
      <Card>
        <SchoolForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={isCreating}
        />
      </Card>
    );
  },
  {
    services: {
      createSchool: {
        path: "schools.create",
        immediate: false,
      },
    },
  },
);
