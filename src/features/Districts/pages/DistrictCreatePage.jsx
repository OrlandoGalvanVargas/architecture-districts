import { PageHader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictCreateController } from "../controllers/DistrictCreateController";

export const DistrictCreatePage = ({ baseBreadcrumbs }) => {
  const addBreadcrumbs = [
    ...baseBreadcrumbs,
    { label: "Create", path: "/districts/create" },
  ];

  return (
    <div>
      <PageHader title="Create New District" breadcrumbs={addBreadcrumbs} />
      <DistrictCreateController />
    </div>
  );
};
