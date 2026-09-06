import { useParams, useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { UserDetail } from "../components/UserDetail";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useUser, useDeleteUser } from "../hooks/useUsers";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";

export const UserDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: user, isLoading, error, refetch } = useUser(id);
  const deleteUserMutation = useDeleteUser();

  const handleEdit = () => {
    navigate(ROUTES.USERS.EDIT(id));
  };

  const handleDelete = () => {
    deleteUserMutation.mutate(id, {
      onSuccess: () => {
        navigate(ROUTES.USERS.LIST);
      },
    });
  };

  const handleBack = () => {
    navigate(ROUTES.USERS.LIST);
  };

  const breadcrumbs = [
    { label: "Users", path: ROUTES.USERS.LIST },
    { label: user?.name || `User #${id}` },
  ];

  if (isLoading) {
    return <LoadingSpinner description="Loading user details..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.USERS.LIST}
        backText="Back to Users"
        subTitle="You do not have permission to view this user."
      />
    );
  }

  if (error?.status === 404) {
    return (
      <NotFoundPage
        backPath={ROUTES.USERS.LIST}
        backText="Back to Users"
        subTitle="User not found."
      />
    );
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={() => refetch()} />;
  }

  return (
    <div>
      <PageHeader title="User Details" breadcrumbs={breadcrumbs} />
      <UserDetail
        user={user}
        isDeleting={deleteUserMutation.isPending}
        onDelete={can(PERMISSIONS.USERS.DELETE) ? handleDelete : null}
        onEdit={can(PERMISSIONS.USERS.UPDATE) ? handleEdit : null}
        onBack={handleBack}
      />
    </div>
  );
};
