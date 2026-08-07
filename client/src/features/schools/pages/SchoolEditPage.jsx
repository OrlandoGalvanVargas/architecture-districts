import { SchoolEditController } from "../controllers/SchoolEditController";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { generateBreadcrumbs } from "@/router/breadcrumbs";
import { useParams } from "react-router-dom";

export const SchoolEditPage = () => {
  const { id } = useParams();
  const breadcrumbs = generateBreadcrumbs("schools", "edit", { id });

  return (
    <div>
      <PageHeader title="Edit School" breadcrumbs={breadcrumbs} />
      <SchoolEditController schoolId={id} />
    </div>
  );
};
