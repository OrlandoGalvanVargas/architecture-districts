import { useParams, useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictDetail } from "../components/DistrictDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useDistrict, useDeleteDistrict } from "../hooks/useDistricts";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";

export const DistrictDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: district, isLoading, error, refetch } = useDistrict(id);
  const deleteDistrictMutation = useDeleteDistrict();

  const handleEdit = () => {
    navigate(ROUTES.DISTRICTS.EDIT(id));
  };

  const handleDelete = () => {
    deleteDistrictMutation.mutate(id, {
      onSuccess: () => {
        navigate(ROUTES.DISTRICTS.LIST);
      },
    });
  };

  const handleBack = () => {
    navigate(ROUTES.DISTRICTS.LIST);
  };

  const breadcrumbs = [
    { label: "Districts", path: ROUTES.DISTRICTS.LIST },
    { label: district?.name || `District #${id}` },
  ];

  if (isLoading) {
    return <LoadingSpinner description="Loading district details..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.DISTRICTS.LIST}
        backText="Back to Districts"
        subTitle="You do not have permission to view this district."
      />
    );
  }

  if (error?.status === 404) {
    return (
      <NotFoundPage
        backPath={ROUTES.DISTRICTS.LIST}
        backText="Back to Districts"
        subTitle="District not found."
      />
    );
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={() => refetch()} />;
  }

  return (
    <div>
      <PageHeader title="District Details" breadcrumbs={breadcrumbs} />
      <DistrictDetail
        district={district}
        isDeleting={deleteDistrictMutation.isPending}
        onDelete={can(PERMISSIONS.DISTRICTS.DELETE) ? handleDelete : null}
        onEdit={can(PERMISSIONS.DISTRICTS.UPDATE) ? handleEdit : null}
        onBack={handleBack}
      />
    </div>
  );
};
