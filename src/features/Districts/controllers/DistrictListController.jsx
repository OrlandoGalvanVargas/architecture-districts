import { useNavigate } from "react-router-dom";
import { Input, Button, Space } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { useNofitication } from "@/contexts/Notification";
import { useState } from "react";
import { useDistricts } from "../hooks/useDistricts";
import { useDistrictDelete } from "../hooks/useDistrictDelete";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { DistrictTable } from "../components/DistrictTable";

const { Search } = Input;

export const DistrictListController = () => {
  const navigate = useNavigate();
  const notification = useNofitication();
  const [searchTerm, setSearchTerm] = useState("");

  const { districts, isLoading, error, refetch } = useDistricts();

  const { deleteDistrict, isDeleting } = useDistrictDelete({
    onSuccess: () => {
      notification.showSuccess("District deleted successfully");
      refetch();
    },
    onError: (error) => {
      notification.showError(error.message);
    },
  });

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
    } catch (error) {
      console.log(error);
    }
  };

  if (isLoading) {
    return <LoadingSpinner description="Loading districts..." />;
  }

  if (error) {
    return <ErrorMessage error={error} onRetry={refetch} />;
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
            <Button icon={<ReloadOutlined />} onClick={refetch}>
              Refetch
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
        districst={filteredDistricts}
        loading={isDeleting}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
    </div>
  );
};
