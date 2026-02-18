import { DistrictCreateController } from "../controllers/DistrictCreateController";
import withPageLayout from "@/HOC/withPageLayout";
import {
  DISTRICT_BREADCRUMBS,
  DISTRICT_BREADCRUMB_CHAINS,
} from "../utils/districtBreadcrumbs";

export const DistrictCreatePage = withPageLayout(DistrictCreateController, {
  title: "Create New District",
  breadcrumbMap: DISTRICT_BREADCRUMBS,
  breadcrumbChain: DISTRICT_BREADCRUMB_CHAINS.create,
});
