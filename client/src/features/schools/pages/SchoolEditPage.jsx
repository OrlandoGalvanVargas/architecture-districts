import { useParams, useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { SchoolForm } from "../components/SchoolForm";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useSchool, useUpdateSchool } from "../hooks/useSchools";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { logger } from "@/services/logger.service";

export const SchoolEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: school, isLoading, error, refetch } = useSchool(id);
  const updateSchoolMutation = useUpdateSchool(id);

  const handleSubmit = async (values) => {
    try {
      await updateSchoolMutation.mutateAsync(values);
      navigate(ROUTES.SCHOOLS.DETAIL(id));
    } catch (error) {
      logger.error("Failed to update school", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.SCHOOLS.DETAIL(id));
  };

  const breadcrumbs = [
    { label: "Schools", path: ROUTES.SCHOOLS.LIST },
    { label: school?.name || `School #${id}`, path: ROUTES.SCHOOLS.DETAIL(id) },
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

  if (!can(PERMISSIONS.SCHOOLS.UPDATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.SCHOOLS.LIST}
        backText="Back to Schools"
        subTitle="You do not have permission to edit schools."
      />
    );
  }

  if (isLoading) {
    return <LoadingSpinner description="Loading school..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.SCHOOLS.LIST}
        backText="Back to Schools"
        subTitle="You do not have permission to edit this school."
      />
    );
  }

  if (error?.status === 404) {
    return (
      <NotFoundPage
        backPath={ROUTES.SCHOOLS.LIST}
        backText="Back to Schools"
        subTitle="School not found."
      />
    );
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={() => refetch()} />;
  }

  return (
    <div>
      <PageHeader
        title="Edit School"
        subtitle="Update the school's information below."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <SchoolForm
          initialValues={school}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={updateSchoolMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
