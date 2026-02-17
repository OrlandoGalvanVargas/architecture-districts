import { PageHader } from "../../../components/Layout/PageHeader/PageHeader";
import { DistrictListController } from "../controllers/DistrictListController";

export const DistrictsPage = () => {
  const breadcrumbs = [
    { label: "Home", path: "/" },
    { label: "Districts", path: "/districts" },
  ];

  return (
    <div>
      <PageHader title="Districts" breadcrumbs={breadcrumbs} />
      <DistrictListController />
    </div>
  );
};
