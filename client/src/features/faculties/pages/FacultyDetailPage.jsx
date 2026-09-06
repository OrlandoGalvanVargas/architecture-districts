import { useParams, useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { FacultyDetail } from "../components/FacultyDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useFaculty, useDeleteFaculty } from "../hooks/useFaculties";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";

export const FacultyDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: faculty, isLoading, error, refetch } = useFaculty(id);
  const deleteFacultyMutation = useDeleteFaculty();

  const handleEdit = () => {
    navigate(ROUTES.FACULTIES.EDIT(id));
  };

  const handleDelete = () => {
    deleteFacultyMutation.mutate(id, {
      onSuccess: () => {
        navigate(ROUTES.FACULTIES.LIST);
      },
    });
  };

  const handleBack = () => {
    navigate(ROUTES.FACULTIES.LIST);
  };

  const breadcrumbs = [
    { label: "Faculties", path: ROUTES.FACULTIES.LIST },
    { label: faculty?.fullName || `Faculty #${id}` },
  ];

  if (isLoading) {
    return <LoadingSpinner description="Loading faculty details..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.FACULTIES.LIST}
        backText="Back to Faculties"
        subTitle="You do not have permission to view this faculty."
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
      <PageHeader title="Faculty Details" breadcrumbs={breadcrumbs} />
      <FacultyDetail
        faculty={faculty}
        isDeleting={deleteFacultyMutation.isPending}
        onDelete={can(PERMISSIONS.FACULTIES.DELETE) ? handleDelete : null}
        onEdit={can(PERMISSIONS.FACULTIES.UPDATE) ? handleEdit : null}
        onBack={handleBack}
      />
    </div>
  );
};
