import { SchoolListController } from "../controllers/SchoolListController";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { generateBreadcrumbs } from "@/router/breadcrumbs";

export const SchoolsPage = () => {
  const breadcrumbs = generateBreadcrumbs("schools", "list");

  return (
    <div>
      <PageHeader title="Schools" breadcrumbs={breadcrumbs} />
      <SchoolListController />
    </div>
  );
};
