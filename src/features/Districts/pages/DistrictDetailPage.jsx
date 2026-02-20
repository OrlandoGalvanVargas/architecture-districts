import { useParams } from "react-router-dom";
import { PageHeader } from "../../../components/Layout/PageHeader/PageHeader";
import { DistrictDetailController } from "../controllers/DistrictDetailController";

export const DistrictDetailPage = () => {
  const { id } = useParams();

  const breadcrumbs = [
    { label: "Home", path: "/" },
    { label: "Districts", path: "/districts" },
    { label: `District #${id}`, path: `districts/${id}` },
  ];

  return (
    <div>
      <PageHeader title="District Details" breadcrumbs={breadcrumbs} />
      <DistrictDetailController />
    </div>
  );
};
