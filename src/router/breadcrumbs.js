import { ROUTES_CONFIG } from "./routes";
import { RoutePaths } from "./RoutePaths";
export const generateBreadcrumbs = (moduleName, viewType, params = {}) => {
  const breadcrumbs = [
    {
      label: ROUTES_CONFIG.home.title,
      path: RoutePaths.home(),
    },
  ];

  const moduleConfig = ROUTES_CONFIG[moduleName];
  if (!moduleConfig) return breadcrumbs;

  breadcrumbs.push({
    label: moduleConfig.title,
    path: RoutePaths[moduleName].list(),
  });

  if (viewType !== "list") {
    const viewConfig = moduleConfig.children[viewType];
    if (!viewConfig) return breadcrumbs;

    if ((viewType === "detail" || viewType === "edit") && params.id) {
      breadcrumbs.push({
        label: params.name
          ? params.name
          : `${moduleConfig.singularTitle || viewConfig.singularTitle} #${params.id}`,
        path: RoutePaths[moduleName].detail(params.id),
      });
    }

    if (viewType === "create") {
      breadcrumbs.push({
        label: viewConfig.title,
        path: RoutePaths[moduleName].create(),
      });
    } else if (viewType === "edit") {
      breadcrumbs.push({
        label: "Edit",
        path: RoutePaths[moduleName].edit(params.id),
      });
    }
  }

  return breadcrumbs;
};
