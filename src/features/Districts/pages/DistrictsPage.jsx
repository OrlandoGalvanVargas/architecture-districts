import { PageHader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictListController } from "../controllers/DistrictListController";

export const DistrictsPage = ({ baseBreadcrumbs }) => {
  return (
    <div>
      <PageHader title="Districts" breadcrumbs={baseBreadcrumbs} />
      <DistrictListController />
    </div>
  );
};
