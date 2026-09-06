import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { UserForm } from "../components/UserForm";
import { useCreateUser } from "../hooks/useUsers";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { logger } from "@/services/logger.service";

export const UserCreatePage = () => {
  const navigate = useNavigate();
  const { can } = usePermission();
  const createUserMutation = useCreateUser();

  if (!can(PERMISSIONS.USERS.CREATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.USERS.LIST}
        backText="Back to Users"
        subTitle="You do not have permission to create users."
      />
    );
  }

  const handleSubmit = async (values) => {
    try {
      await createUserMutation.mutateAsync(values);
      navigate(ROUTES.USERS.LIST);
    } catch (error) {
      logger.error("Failed to create user", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.USERS.LIST);
  };

  const breadcrumbs = [{ label: "Users", path: ROUTES.USERS.LIST }, { label: "Create User" }];

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
        title="Create User"
        subtitle="Create a new user account and assign its role and entity."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <UserForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={createUserMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
