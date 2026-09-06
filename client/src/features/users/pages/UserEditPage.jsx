import { useParams, useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { UserForm } from "../components/UserForm";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useUser, useUpdateUser } from "../hooks/useUsers";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { logger } from "@/services/logger.service";

export const UserEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { can } = usePermission();

  const { data: user, isLoading, error, refetch } = useUser(id);
  const updateUserMutation = useUpdateUser(id);

  const handleSubmit = async (values) => {
    try {
      await updateUserMutation.mutateAsync(values);
      navigate(ROUTES.USERS.DETAIL(id));
    } catch (error) {
      logger.error("Failed to update user", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.USERS.DETAIL(id));
  };

  const breadcrumbs = [
    { label: "Users", path: ROUTES.USERS.LIST },
    { label: user?.name || `User #${id}`, path: ROUTES.USERS.DETAIL(id) },
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

  if (!can(PERMISSIONS.USERS.UPDATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.USERS.LIST}
        backText="Back to Users"
        subTitle="You do not have permission to edit users."
      />
    );
  }

  if (isLoading) {
    return <LoadingSpinner description="Loading user..." />;
  }

  if (error?.status === 403) {
    return (
      <ForbiddenPage
        backPath={ROUTES.USERS.LIST}
        backText="Back to Users"
        subTitle="You do not have permission to edit this user."
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
      <PageHeader
        title="Edit User"
        subtitle="Update the user's information below."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <UserForm
          initialValues={user}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={updateUserMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
