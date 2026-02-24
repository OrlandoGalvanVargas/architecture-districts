import { useParams } from "react-router-dom";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { DistrictEditController } from "../controllers/DistrictEditController";
import { generateBreadcrumbs } from "@/router/breadcrumbs";

export const DistrictEditPage = () => {
  const { id } = useParams();
  const breadcrumbs = generateBreadcrumbs("districts", "edit", { id });

  return (
    <div>
      <PageHeader title="Edit District" breadcrumbs={breadcrumbs} />
      <DistrictEditController districtId={id} />
    </div>
  );
};
