import { useParams } from "react-router-dom";
import { PageHeader } from "../../../components/Layout/PageHeader/PageHeader";
import { DistrictEditController } from "../controllers/DistrictEditController";

export const DistrictEditPage = () => {
  const { id } = useParams();

  const breadcrumbs = [
    { label: "Home", path: "/" },
    { label: "Districts", path: "/districts" },
    { label: `District #${id}`, path: `/districts/${id}` },
    { label: "Edit", path: `/districts/${id}/edit` },
  ];

  return (
    <div>
      <PageHeader title="Edit District" breadcrumbs={breadcrumbs} />
      <DistrictEditController />
    </div>
  );
};
