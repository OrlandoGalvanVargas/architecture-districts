import { useParams, useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { BeaconForm } from "../components/BeaconForm";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useBeacon, useUpdateBeacon } from "../hooks/useBeacons";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { logger } from "@/services/logger.service";

export const BeaconEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: beacon, isLoading, error, refetch } = useBeacon(id);
  const updateBeaconMutation = useUpdateBeacon(id);

  const handleSubmit = async (values) => {
    try {
      await updateBeaconMutation.mutateAsync(values);
      navigate(ROUTES.BEACONS.DETAIL(id));
    } catch (error) {
      logger.error("Failed to update beacon", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.BEACONS.DETAIL(id));
  };

  const breadcrumbs = [
    { label: "Beacons", path: ROUTES.BEACONS.LIST },
    { label: beacon?.deviceName || `Beacon #${id}`, path: ROUTES.BEACONS.DETAIL(id) },
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

  if (!can(PERMISSIONS.BEACONS.UPDATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.BEACONS.LIST}
        backText="Back to Beacons"
        subTitle="You do not have permission to edit beacons."
      />
    );
  }

  if (isLoading) {
    return <LoadingSpinner description="Loading beacon..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.BEACONS.LIST}
        backText="Back to Beacons"
        subTitle="You do not have permission to edit this beacon."
      />
    );
  }

  if (error?.status === 404) {
    return (
      <NotFoundPage
        backPath={ROUTES.BEACONS.LIST}
        backText="Back to Beacons"
        subTitle="Beacon not found."
      />
    );
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={() => refetch()} />;
  }

  return (
    <div>
      <PageHeader
        title="Edit Beacon"
        subtitle="Update the beacon's information below."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <BeaconForm
          initialValues={beacon}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={updateBeaconMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
