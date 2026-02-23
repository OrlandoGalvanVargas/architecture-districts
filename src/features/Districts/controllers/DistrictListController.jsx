// features/districts/controllers/DistrictListController.jsx
import { withController } from "@/reactive/withController";
import { DistrictTable } from "../components/DistrictTable";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { Input, Button, Space } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import useNotification from "antd/es/notification/useNotification";

export const DistrictListController = withController(
  ({ data, loading, errors, actions }) => {
    const navigate = useNavigate();
    const { Search } = Input;
    const notification = useNotification();
    const [searchTerm, setSearchTerm] = useState("");

    const districts = data.districts || [];

    const isLoadingDistricts = loading.districts;
    const isDeletingDistrict = loading.deleteDistrict;

    const fetchError = errors.districts;

    const refetchDistricts = actions.districts;
    const deleteDistrict = actions.deleteDistrict;

    const filteredDistricts = districts.filter(
      (district) =>
        district.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        district.code.toLowerCase().includes(searchTerm.toLowerCase()),
    );

    const handleView = (district) => {
      navigate(`/districts/${district.id}`);
    };

    const handleCreate = () => {
      navigate("/districts/create");
    };

    const handleEdit = (district) => {
      navigate(`/districts/${district.id}/edit`);
    };

    const handleDelete = async (district) => {
      try {
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
              onChange={(e) => setSearchTerm(e.target.value)}
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
        params: [],
        onSuccess: (data) => {
          console.log("Districts loaded:", data.length);
        },
        onError: (error) => {
          console.error("Failed to load districts:", error);
        },
      },

      deleteDistrict: {
        path: "districts.delete",
        immediate: false,
      },
    },
  },
);
