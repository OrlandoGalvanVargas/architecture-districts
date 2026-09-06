import { useParams, useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

import { BeaconDetail } from "../components/BeaconDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useBeacon, useDeleteBeacon } from "../hooks/useBeacons";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";

export const BeaconDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: beacon, isLoading, error, refetch } = useBeacon(id);
  const deleteBeaconMutation = useDeleteBeacon();

  const handleEdit = () => {
    navigate(ROUTES.BEACONS.EDIT(id));
  };

  const handleDelete = () => {
    deleteBeaconMutation.mutate(id, {
      onSuccess: () => {
        navigate(ROUTES.BEACONS.LIST);
      },
    });
  };

  const handleBack = () => {
    navigate(ROUTES.BEACONS.LIST);
  };

  const breadcrumbs = [
    { label: "Beacons", path: ROUTES.BEACONS.LIST },
    { label: beacon?.deviceName || `Beacon #${id}` },
  ];

  if (isLoading) {
    return <LoadingSpinner description="Loading beacon details..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.BEACONS.LIST}
        backText="Back to Beacons"
        subTitle="You do not have permission to view this beacon."
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
      <PageHeader title="Beacon Details" breadcrumbs={breadcrumbs} />
      <BeaconDetail
        beacon={beacon}
        isDeleting={deleteBeaconMutation.isPending}
        onDelete={can(PERMISSIONS.BEACONS.DELETE) ? handleDelete : null}
        onEdit={can(PERMISSIONS.BEACONS.UPDATE) ? handleEdit : null}
        onBack={handleBack}
      />
    </div>
  );
};
