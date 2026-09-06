import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { BeaconForm } from "../components/BeaconForm";
import { useCreateBeacon } from "../hooks/useBeacons";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { logger } from "@/services/logger.service";

export const BeaconCreatePage = () => {
  const navigate = useNavigate();
  const { can } = usePermission();
  const createBeaconMutation = useCreateBeacon();

  if (!can(PERMISSIONS.BEACONS.CREATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.BEACONS.LIST}
        backText="Back to Beacons"
        subTitle="You do not have permission to create beacons."
      />
    );
  }

  const handleSubmit = async (values) => {
    try {
      await createBeaconMutation.mutateAsync(values);
      navigate(ROUTES.BEACONS.LIST);
    } catch (error) {
      logger.error("Failed to create beacon", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.BEACONS.LIST);
  };

  const breadcrumbs = [{ label: "Beacons", path: ROUTES.BEACONS.LIST }, { label: "Create Beacon" }];

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
        title="Create Beacon"
        subtitle="Register a new beacon device and optionally assign it to a district or school."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <BeaconForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={createBeaconMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
