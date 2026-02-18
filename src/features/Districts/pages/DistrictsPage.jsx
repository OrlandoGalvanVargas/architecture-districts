import { DistrictListController } from "../controllers/DistrictListController";
import withPageLayout from "@/HOC/withPageLayout";
import {
  DISTRICT_BREADCRUMBS,
  DISTRICT_BREADCRUMB_CHAINS,
} from "../utils/districtBreadcrumbs";

export const DistrictListPage = withPageLayout(DistrictListController, {
  title: "Districts",
  breadcrumbMap: DISTRICT_BREADCRUMBS,
  breadcrumbChain: DISTRICT_BREADCRUMB_CHAINS.list,
});
