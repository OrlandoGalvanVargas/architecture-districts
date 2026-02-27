import { DistrictCreateController } from "../controllers/DistrictCreateController";
import { generateBreadcrumbs } from "@/router/breadcrumbs";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

export const DistrictCreatePage = () => {
  const breadcrumbs = generateBreadcrumbs("districts", "create");

  return (
    <div>
      <PageHeader title="Create New District" breadcrumbs={breadcrumbs} />
      <DistrictCreateController />
    </div>
  );
};
