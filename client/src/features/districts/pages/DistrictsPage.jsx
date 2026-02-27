import { DistrictListController } from "../controllers/DistrictListController";
import { generateBreadcrumbs } from "@/router/breadcrumbs";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

export const DistrictListPage = () => {
  const breadcrumbs = generateBreadcrumbs("districts", "list");

  return (
    <div>
      <PageHeader title="Districts" breadcrumbs={breadcrumbs} />
      <DistrictListController />
    </div>
  );
};
