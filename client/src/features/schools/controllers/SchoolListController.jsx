import { withController } from "@/reactive/withController";
import { SchoolTable } from "../components/SchoolTable";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useEffect, useState } from "react";
import { Input, Button, Space } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "../../../hooks/useAppNavigation";

export const SchoolListController = withController(
  ({ data, loading, errors, actions, setCallbacks }) => {
    const schools = data.schools || [];
    const isLoadingSchools = loading.schools;
    const isDeletingSchool = loading.deleteSchool;
    const fetchError = errors.schools;
    const refetchSchools = actions.schools;
    const deleteSchool = actions.deleteSchool;

    const [search, setSearch] = useState("");
    const { Search } = Input;
    const navigation = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("deleteSchool", {
        onSuccess: () => {
          notification.showSuccess("School deleted successfully");
          refetchSchools();
        },
        onError: (error) => {
          notification.showError(error);
        },
      });
    }, [setCallbacks, refetchSchools, notification]);

    const handleCreate = () => {
      navigation.goToSchoolCreate();
    };

    const handleView = (school) => {
      navigation.goToSchoolDetail(school.id);
    };

    const handleEdit = (school) => {
      navigation.goToSchoolEdit(school.id);
    };

    const handleDelete = (school) => {
      deleteSchool(school.id);
    };

    const filteredSchools = schools.filter(
      (school) =>
        school.name.toLowerCase().includes(search.toLowerCase()) ||
        school.schoolCode.toLowerCase().includes(search.toLowerCase()),
    );

    if (isLoadingSchools) {
      return <LoadingSpinner description="Loading schools..." />;
    }

    if (fetchError) {
      return <ErrorMessage error={fetchError} onRetry={refetchSchools} />;
    }

    return (
      <div>
        <div style={{ marginBottom: 16 }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <Search
              placeholder="Search schools by name or code..."
              allowClear
              style={{ width: 400 }}
              onChange={(e) => setSearch(e.target.value)}
              disabled={isDeletingSchool}
            />
            <Space>
              <Button
                icon={<ReloadOutlined />}
                onClick={refetchSchools}
                disabled={isDeletingSchool}
              >
                Refresh
              </Button>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={handleCreate}
                disabled={isDeletingSchool}
              >
                Create School
              </Button>
            </Space>
          </Space>
        </div>
        <SchoolTable
          schools={filteredSchools}
          loading={isLoadingSchools}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      </div>
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
