import { useParams, useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { FacultyForm } from "../components/FacultyForm";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useFaculty, useUpdateFaculty } from "../hooks/useFaculties";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { logger } from "@/services/logger.service";

export const FacultyEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: faculty, isLoading, error, refetch } = useFaculty(id);
  const updateFacultyMutation = useUpdateFaculty(id);

  const handleSubmit = async (values) => {
    try {
      await updateFacultyMutation.mutateAsync(values);
      navigate(ROUTES.FACULTIES.DETAIL(id));
    } catch (error) {
      logger.error("Failed to update faculty", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.FACULTIES.DETAIL(id));
  };

  const breadcrumbs = [
    { label: "Faculties", path: ROUTES.FACULTIES.LIST },
    { label: faculty?.fullName || `Faculty #${id}`, path: ROUTES.FACULTIES.DETAIL(id) },
    { label: "Edit" },
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

  if (!can(PERMISSIONS.FACULTIES.UPDATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.FACULTIES.LIST}
        backText="Back to Faculties"
        subTitle="You do not have permission to edit faculty."
      />
    );
  }

  if (isLoading) {
    return <LoadingSpinner description="Loading faculty..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.FACULTIES.LIST}
        backText="Back to Faculties"
        subTitle="You do not have permission to edit this faculty."
      />
    );
  }

  if (error?.status === 404) {
    return (
      <NotFoundPage
        backPath={ROUTES.FACULTIES.LIST}
        backText="Back to Faculties"
        subTitle="Faculty not found."
      />
    );
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={() => refetch()} />;
  }

  return (
    <div>
      <PageHeader
        title="Edit Faculty"
        subtitle="Update the faculty's information below."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <FacultyForm
          initialValues={faculty}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={updateFacultyMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
