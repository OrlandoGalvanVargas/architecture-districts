import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";

function withPageLayout(Controller, options = {}) {
  return (props) => {
    const { title = null, breadcrumbMap = {}, breadcrumbChain = [] } = options;

    const breadcrumbs = breadcrumbChain.map((key) => {
      const crumb = breadcrumbMap[key];
      return {
        label: crumb.label,
        path: crumb.path,
      };
    });

    return (
      <div>
        <PageHeader title={title} breadcrumbs={breadcrumbs} />
        <Controller {...props} />
      </div>
    );
  };
}

export default withPageLayout;
