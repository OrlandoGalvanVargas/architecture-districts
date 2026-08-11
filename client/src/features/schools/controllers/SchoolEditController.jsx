import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useNotification } from "@/contexts/Notification";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { SchoolForm } from "../components/SchoolForm";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect } from "react";

export const SchoolEditController = withController(
  ({ data, loading, errors, actions, schoolId, setCallbacks }) => {
    const school = data.school;
    const isLoading = loading.school;
    const isUpdating = loading.updateSchool;
    const error = errors.school;
    const updateSchool = actions.updateSchool;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("updateSchool", {
        onSuccess: () => {
          notification.showSuccess("School update successfully");
          navigate.goToSchoolDetail(schoolId);
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [setCallbacks, schoolId]);

    const handleSubmit = (values) => {
      updateSchool(schoolId, values);
    };

    const handleCancel = () => {
      navigate.goToSchoolDetail(schoolId);
    };

    if (isLoading) {
      return <LoadingSpinner description="Loading school..." />;
    }

    if (error) {
      return <ErrorMessage error={error} />;
    }

    return (
      <Card>
        <SchoolForm
          initialValues={school}
          loading={isUpdating}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
        />
      </Card>
    );
  },
  {
    services: {
      school: {
        path: "schools.getById",
        immediate: false,
      },
      updateSchool: {
        path: "schools.update",
        immediate: false,
      },
    },

    init: ({ actions, props }) => {
      if (props.schoolId && actions.school) {
        actions.school(props.schoolId);
      }
    },
  },
);
