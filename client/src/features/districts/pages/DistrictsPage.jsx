import { useState } from "react";
import { Input, Button } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { TableCard } from "../../../components/common/TableCard/TableCard";
import { DistrictTable } from "../components/DistrictTable";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";
import { useDistricts, useDeleteDistrict } from "../hooks/useDistricts";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import "./DistrictsPage.css";

const { Search } = Input;

export const DistrictsPage = () => {
  const [search, setSearch] = useState("");
  const navigation = useAppNavigation();
  const { can } = usePermission();

  const { data: districtsData, isLoading, isFetching, error, refetch } = useDistricts();
  const deleteDistrictMutation = useDeleteDistrict();

  const handleView = (district) => navigation.districts.detail(district.id);
  const handleCreate = () => navigation.districts.create();
  const handleEdit = (district) => navigation.districts.edit(district.id);
  const handleDelete = (district) => deleteDistrictMutation.mutate(district.id);

  const breadcrumbs = [{ label: "Districts", path: "/districts" }];

  return (
    <div>
      <PageHeader
        title="Districts"
        breadcrumbs={breadcrumbs}
        extra={
          can(PERMISSIONS.DISTRICTS.CREATE) && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
              Create District
            </Button>
          )
        }
      />

      <div className="toolbar">
        <Search
          placeholder="Search districts by name or code..."
          allowClear
          className="toolbar-search"
          onChange={(e) => setSearch(e.target.value)}
        />
        <Button
          icon={<ReloadOutlined />}
          onClick={refetch}
          loading={isFetching}
          disabled={isFetching}
        >
          Refresh
        </Button>
      </div>

      <QueryStateHandler
        isLoading={isLoading}
        error={error}
        data={districtsData}
        refetch={refetch}
        loadingDescription="Loading districts..."
      >
        {(data) => {
          const districts = data || [];
          const filteredDistricts = districts.filter(
            (district) =>
              district.name?.toLowerCase().includes(search.toLowerCase()) ||
              district.code?.toLowerCase().includes(search.toLowerCase())
          );

          return (
            <TableCard>
              <DistrictTable
                districts={filteredDistricts}
                loading={deleteDistrictMutation.isPending || isFetching}
                onView={handleView}
                onEdit={can(PERMISSIONS.DISTRICTS.UPDATE) ? handleEdit : null}
                onDelete={can(PERMISSIONS.DISTRICTS.DELETE) ? handleDelete : null}
              />
            </TableCard>
          );
        }}
      </QueryStateHandler>
    </div>
  );
};
