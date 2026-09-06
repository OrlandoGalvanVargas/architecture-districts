import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { FacultyForm } from "../components/FacultyForm";
import { useCreateFaculty } from "../hooks/useFaculties";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { logger } from "@/services/logger.service";

export const FacultyCreatePage = () => {
  const navigate = useNavigate();
  const { can } = usePermission();
  const createFacultyMutation = useCreateFaculty();

  if (!can(PERMISSIONS.FACULTIES.CREATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.FACULTIES.LIST}
        backText="Back to Faculties"
        subTitle="You do not have permission to create faculty."
      />
    );
  }

  const handleSubmit = async (values) => {
    try {
      await createFacultyMutation.mutateAsync(values);
      navigate(ROUTES.FACULTIES.LIST);
    } catch (error) {
      logger.error("Failed to create faculty", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.FACULTIES.LIST);
  };

  const breadcrumbs = [
    { label: "Faculties", path: ROUTES.FACULTIES.LIST },
    { label: "Create Faculty" },
  ];

  const apiErrorMapper = (error) => {
    if (error?.details?.errors) {
      const fieldErrors = {};
      Object.entries(error.details.errors).forEach(([field, messages]) => {
        fieldErrors[field] = Array.isArray(messages) ? messages[0] : messages;
      });
      return fieldErrors;
    }
    return null;
  };

  return (
    <div>
      <PageHeader
        title="Create Faculty"
        subtitle="Add a new faculty member and assign them to a district or school."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <FacultyForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={createFacultyMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
