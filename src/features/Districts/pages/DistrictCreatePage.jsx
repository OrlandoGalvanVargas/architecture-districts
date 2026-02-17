import { PageHader } from "../../../components/Layout/PageHeader/PageHeader";
import { DistrictCreateController } from "../controllers/DistrictCreateController";

export const DistrictCreatePage = () => {
  const breadcrumbs = [
    { label: "home", path: "/" },
    { label: "Districts", path: "/districts" },
    { label: "create", path: "/districts/create" },
  ];

  return (
    <div>
      <PageHader title="Create New District" breadcrumbs={breadcrumbs} />
      <DistrictCreateController />
    </div>
  );
};
