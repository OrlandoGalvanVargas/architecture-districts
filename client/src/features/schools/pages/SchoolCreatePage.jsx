import { useNavigate } from "react-router-dom";
import { Card } from "antd";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { SchoolForm } from "../components/SchoolForm";
import { useCreateSchool } from "../hooks/useSchools";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { logger } from "@/services/logger.service";

export const SchoolCreatePage = () => {
  const navigate = useNavigate();
  const { can } = usePermission();
  const createSchoolMutation = useCreateSchool();

  if (!can(PERMISSIONS.SCHOOLS.CREATE)) {
    return (
      <ForbiddenPage
        backPath={ROUTES.SCHOOLS.LIST}
        backText="Back to Schools"
        subTitle="You do not have permission to create schools."
      />
    );
  }

  const handleSubmit = async (values) => {
    try {
      await createSchoolMutation.mutateAsync(values);
      navigate(ROUTES.SCHOOLS.LIST);
    } catch (error) {
      logger.error("Failed to create school", error);
      throw error;
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.SCHOOLS.LIST);
  };

  const breadcrumbs = [{ label: "Schools", path: ROUTES.SCHOOLS.LIST }, { label: "Create School" }];

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
        title="Create School"
        subtitle="Add a new school and link it to its district."
        breadcrumbs={breadcrumbs}
      />
      <Card>
        <SchoolForm
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          loading={createSchoolMutation.isPending}
          apiErrorMapper={apiErrorMapper}
        />
      </Card>
    </div>
  );
};
