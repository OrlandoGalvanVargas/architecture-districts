import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useNotification } from "@/contexts/Notification";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { SchoolForm } from "../components/SchoolForm";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { useEffect, useMemo } from "react";

const LEVEL_MAP = { Elementary: 0, Middle: 1, High: 2, K12: 3, Prek: 4 };
const TYPE_MAP = { Public: 0, Charter: 1, Magnet: 2, Alternative: 3 };

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
    }, [setCallbacks, schoolId, navigate, notification]);

    const normalizedSchool = useMemo(() => {
      if (!school) return null;
      return {
        ...school,
        level:
          typeof school.level === "string"
            ? (LEVEL_MAP[school.level] ?? 0)
            : school.level,
        type:
          typeof school.type === "string"
            ? (TYPE_MAP[school.type] ?? 0)
            : school.type,
      };
    }, [school]);

    const handleSubmit = (values) => {
      const payload = {
        ...values,
        level:
          typeof values.level === "string"
            ? (LEVEL_MAP[values.level] ?? 0)
            : Number(values.level),
        type:
          typeof values.type === "string"
            ? (TYPE_MAP[values.type] ?? 0)
            : Number(values.type),
      };
      updateSchool(schoolId, payload);
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
          initialValues={normalizedSchool}
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
