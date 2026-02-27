import { withController } from "@/reactive/withController";
import { DistrictTable } from "../components/DistrictTable";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useEffect, useState } from "react";
import { Input, Button, Space } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const DistrictListController = withController(
  ({ data, loading, errors, actions, setCallbacks }) => {
    const districts = data.districts || [];
    const isLoadingDistricts = loading.districts;
    const isDeletingDistrict = loading.deleteDistrict;
    const fetchError = errors.districts;
    const refetchDistricts = actions.districts;
    const deleteDistrict = actions.deleteDistrict;

    const [search, setSearch] = useState("");
    const { Search } = Input;
    const navigation = useAppNavigation();
    const notification = useNotification();

    useEffect(() => {
      setCallbacks("deleteDistrict", {
        onSuccess: () => {
          notification.showSuccess("District deleted successfully");
          refetchDistricts();
        },
        onError: (error) => {
          notification.showError(error.message);
        },
      });
    }, [refetchDistricts, setCallbacks]);

    const handleView = (district) => {
      navigation.goToDistrictDetail(district.id);
    };

    const handleCreate = () => {
      navigation.goToDistrictCreate();
    };

    const handleEdit = (district) => {
      navigation.goToDistrictEdit(district.id);
    };

    const handleDelete = (district) => {
      deleteDistrict(district.id);
    };

    const filteredDistricts = districts.filter(
      (district) =>
        district.name.toLowerCase().includes(search.toLowerCase()) ||
        district.code.toLowerCase().includes(search.toLowerCase()),
    );

    if (isLoadingDistricts) {
      return <LoadingSpinner description="Loading districts..." />;
    }

    if (fetchError) {
      return <ErrorMessage error={fetchError} onRetry={refetchDistricts} />;
    }

    return (
      <div>
        <div style={{ marginBottom: 16 }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <Search
              placeholder="Search districts by name or code..."
              allowClear
              style={{ width: 400 }}
              onChange={(e) => setSearch(e.target.value)}
              disabled={isDeletingDistrict}
            />
            <Space>
              <Button
                icon={<ReloadOutlined />}
                onClick={refetchDistricts}
                disabled={isDeletingDistrict}
              >
                Refresh
              </Button>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={handleCreate}
                disabled={isDeletingDistrict}
              >
                Create District
              </Button>
            </Space>
          </Space>
        </div>
        <DistrictTable
          districts={filteredDistricts}
          loading={isDeletingDistrict}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      </div>
    );
  },
  {
    services: {
      districts: {
        path: "districts.getAll",
        immediate: true,
      },

      deleteDistrict: {
        path: "districts.delete",
        immediate: false,
      },
    },
  },
);
