import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictForm } from "../components/DistrictForm";
import { useCreateDistrict } from "../hooks/useDistricts";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { logger } from "@/services/logger.service";

export const DistrictCreatePage = () => {
  const navigate = useNavigate();
  const { can } = usePermission();
  const createDistrictMutation = useCreateDistrict();

  if (!can(PERMISSIONS.DISTRICTS.CREATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.DISTRICTS.LIST}
        backText="Back to Districts"
        subTitle="You do not have permission to create districts."
      />
    );
  }

  const handleSubmit = async (values) => {
    try {
      await createDistrictMutation.mutateAsync(values);
      navigate(ROUTES.DISTRICTS.LIST);
    } catch (error) {
      logger.error("Failed to create district", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.DISTRICTS.LIST);
  };

  const breadcrumbs = [
    { label: "Districts", path: ROUTES.DISTRICTS.LIST },
    { label: "Create District" },
  ];

  return (
    <div>
      <PageHeader
        title="Create District"
        subtitle="Add a new school district to start organizing schools underneath it."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <DistrictForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={createDistrictMutation.isPending}
        />
      </Card>
    </div>
  );
};
