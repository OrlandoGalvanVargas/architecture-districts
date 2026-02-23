import { useParams } from "react-router-dom";
import { PageHeader } from "../../../components/Layout/PageHeader/PageHeader";
import { DistrictDetailController } from "../controllers/DistrictDetailController";
import { generateBreadcrumbs } from "@/router/breadcrumbs";

export const DistrictDetailPage = () => {
  const { id } = useParams();
  const breadcrumbs = generateBreadcrumbs("districts", "detail", { id });

  return (
    <div>
      <PageHeader title="District Details" breadcrumbs={breadcrumbs} />
      <DistrictDetailController districtId={id} />
    </div>
  );
};
