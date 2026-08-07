import SchoolCreateController from "../controllers/SchoolCreateController";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { generateBreadcrumbs } from "@/router/breadcrumbs";

export const SchoolCreatePage = () => {
  const breadcrumbs = generateBreadcrumbs("schools", "create");

  return (
    <div>
      <PageHeader title="Create School" breadcrumbs={breadcrumbs} />
      <SchoolCreateController />
    </div>
  );
};
