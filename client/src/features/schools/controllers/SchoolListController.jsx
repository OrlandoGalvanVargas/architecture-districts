import { withController } from "@/reactive/withController";
import { SchoolTable } from "../components/SchoolTable";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useEffect, useState } from "react";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const SchoolListController = withController(
  ({ data, loading, errors, actions, setCallbacks }) => {
    // PagedResult de C#
    const responseData = data.schools || {
      items: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
    };
    const schools = responseData.items || [];

    const isLoadingSchools = loading.schools;
    const isDeletingSchool = loading.deleteSchool;
    const fetchError = errors.schools;
    const refetchSchools = actions.schools;
    const deleteSchool = actions.deleteSchool;

    // Estado inicial alineado con la Query de C#
    const [filters, setFilters] = useState({
      page: 1,
      pageSize: 10,
      isActive: true,
    });
    const navigation = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("deleteSchool", {
        onSuccess: () => {
          notification.showSuccess("School deleted successfully");
          refetchSchools(filters);
        },
        onError: (error) => {
          notification.showError(error.message || "Error deleting school");
        },
      });
    }, [setCallbacks, refetchSchools, notification, filters]);

    const handleFilterChange = (newFilters) => {
      const updatedFilters = { ...filters, ...newFilters, page: 1 };
      setFilters(updatedFilters);
      refetchSchools(updatedFilters);
    };

    const handlePageChange = (page, pageSize) => {
      const updatedFilters = { ...filters, page, pageSize };
      setFilters(updatedFilters);
      refetchSchools(updatedFilters);
    };

    const handleCreate = () => navigation.goToSchoolCreate();
    const handleView = (school) => navigation.goToSchoolDetail(school.id);
    const handleEdit = (school) => navigation.goToSchoolEdit(school.id);
    const handleDelete = (school) => deleteSchool(school.id);

    if (isLoadingSchools && !schools.length) {
      return <LoadingSpinner description="Loading schools..." />;
    }

    if (fetchError) {
      return (
        <ErrorMessage
          error={fetchError}
          onRetry={() => refetchSchools(filters)}
        />
      );
    }

    return (
      <SchoolTable
        schools={schools}
        loading={isLoadingSchools || isDeletingSchool}
        pagination={{
          current: responseData.page || filters.page,
          pageSize: responseData.pageSize || filters.pageSize,
          total: responseData.totalCount || 0,
        }}
        onPageChange={handlePageChange}
        onFilterChange={handleFilterChange}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onCreate={handleCreate}
        onRefresh={() => refetchSchools(filters)}
      />
    );
  },
  {
    services: {
      schools: {
        path: "schools.getAll",
        immediate: true,
      },
      deleteSchool: {
        path: "schools.delete",
        immediate: false,
      },
    },
  },
);
