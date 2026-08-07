import SchoolDetailController from "../controllers/SchoolDetailController";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { generateBreadcrumbs } from "@/router/breadcrumbs";
import { useParams } from "react-router-dom";

export const SchoolDetailPage = () => {
  const { id } = useParams();
  const breadcrumbs = generateBreadcrumbs("schools", "detail", { id });

  return (
    <div>
      <PageHeader title="School Details" breadcrumbs={breadcrumbs} />
      <SchoolDetailController schoolId={id} />
    </div>
  );
};
