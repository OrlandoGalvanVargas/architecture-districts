import { withController } from "@/reactive/withController";
import { useNotification } from "@/contexts/Notification";
import { SchoolDetail } from "../components/SchoolDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect } from "react";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";

export const SchoolDetailController = withController(
  ({ data, loading, errors, actions, schoolId, setCallbacks }) => {
    const school = data.school;
    const isLoading = loading.school;
    const isDeleting = loading.deleteSchool;
    const error = errors.school;
    const refetchSchool = actions.school;
    const deleteSchool = actions.deleteSchool;

    const navigate = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("deleteSchool", {
        onSuccess: () => {
          notification.showSuccess("School deleted successfully");
          navigate.goToSchools();
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [setCallbacks]);

    const handleEdit = () => {
      navigate.goToSchoolEdit(schoolId);
    };

    const handleDelete = () => {
      deleteSchool(schoolId);
    };

    const handleBack = () => {
      navigate.goToSchools();
    };

    if (isLoading) {
      return <LoadingSpinner description="Loading school details..." />;
    }

    if (error) {
      return (
        <ErrorMessage error={error} onRetry={() => refetchSchool(schoolId)} />
      );
    }

    return (
      <SchoolDetail
        school={school}
        isDeleting={isDeleting}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onBack={handleBack}
      />
    );
  },
  {
    services: {
      school: {
        path: "schools.getById",
        immediate: false,
      },
      deleteSchool: {
        path: "schools.delete",
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
