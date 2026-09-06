import { useParams, useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { SchoolDetail } from "../components/SchoolDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useSchool, useDeleteSchool } from "../hooks/useSchools";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";

export const SchoolDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: school, isLoading, error, refetch } = useSchool(id);
  const deleteSchoolMutation = useDeleteSchool();

  const handleEdit = () => {
    navigate(ROUTES.SCHOOLS.EDIT(id));
  };

  const handleDelete = () => {
    deleteSchoolMutation.mutate(id, {
      onSuccess: () => {
        navigate(ROUTES.SCHOOLS.LIST);
      },
    });
  };

  const handleBack = () => {
    navigate(ROUTES.SCHOOLS.LIST);
  };

  const breadcrumbs = [
    { label: "Schools", path: ROUTES.SCHOOLS.LIST },
    { label: school?.name || `School #${id}` },
  ];

  if (isLoading) {
    return <LoadingSpinner description="Loading school details..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.SCHOOLS.LIST}
        backText="Back to Schools"
        subTitle="You do not have permission to view this school."
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
        title="School Details"
        subtitle="Update the school's information below."
        breadcrumbs={breadcrumbs}
      />
      <SchoolDetail
        school={school}
        isDeleting={deleteSchoolMutation.isPending}
        onDelete={can(PERMISSIONS.SCHOOLS.DELETE) ? handleDelete : null}
        onEdit={can(PERMISSIONS.SCHOOLS.UPDATE) ? handleEdit : null}
        onBack={handleBack}
      />
    </div>
  );
};
