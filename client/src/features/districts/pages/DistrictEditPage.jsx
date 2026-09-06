import { useParams, useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictForm } from "../components/DistrictForm";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useDistrict, useUpdateDistrict } from "../hooks/useDistricts";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { logger } from "@/services/logger.service";

export const DistrictEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: district, isLoading, error, refetch } = useDistrict(id);
  const updateDistrictMutation = useUpdateDistrict(id);

  const handleSubmit = async (values) => {
    try {
      await updateDistrictMutation.mutateAsync(values);
      navigate(ROUTES.DISTRICTS.DETAIL(id));
    } catch (error) {
      logger.error("Failed to update district", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.DISTRICTS.DETAIL(id));
  };

  const breadcrumbs = [
    { label: "Districts", path: ROUTES.DISTRICTS.LIST },
    { label: district?.name || `District #${id}`, path: ROUTES.DISTRICTS.DETAIL(id) },
    { label: "Edit" },
  ];

  if (!can(PERMISSIONS.DISTRICTS.UPDATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.DISTRICTS.LIST}
        backText="Back to Districts"
        subTitle="You do not have permission to edit districts."
      />
    );
  }

  if (isLoading) {
    return <LoadingSpinner description="Loading district..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.DISTRICTS.LIST}
        backText="Back to Districts"
        subTitle="You do not have permission to edit this district."
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
      <PageHeader
        title="Edit District"
        subtitle="Update the district's information below."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <DistrictForm
          initialValues={district}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={updateDistrictMutation.isPending}
        />
      </Card>
    </div>
  );
};
