import { withController } from "@/reactive/withController";
import { DistrictTable } from "../components/DistrictTable";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useState } from "react";
import { Input, Button, Space } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const DistrictListController = withController(
  ({ data, loading, errors, actions }) => {
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

    const filteredDistricts = districts.filter(
      (district) =>
        district.name.toLowerCase().includes(search.toLowerCase()) ||
        district.code.toLowerCase().includes(search.toLowerCase()),
    );

    const handleView = (district) => {
      navigation.goToDistrictDetail(district.id);
    };

    const handleCreate = () => {
      navigation.goToDistrictCreate();
    };

    const handleEdit = (district) => {
      navigation.goToDistrictEdit(district.id);
    };

    const handleDelete = async (district) => {
      try {
        console.log(district.id);
        await deleteDistrict(district.id);
        notification.showSuccess("District deleted successfully");
        refetchDistricts();
      } catch (error) {
        notification.showError(error.message);
      }
    };

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
            />
            <Space>
              <Button icon={<ReloadOutlined />} onClick={refetchDistricts}>
                Refresh
              </Button>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={handleCreate}
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
