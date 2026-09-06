import { useState } from "react";
import { Input, Button, Space, Select, Switch } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { BeaconTable } from "../components/BeaconTable";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";
import { useBeacons, useDeleteBeacon } from "../hooks/useBeacons";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { BEACON_TYPES, BEACON_STATUSES } from "../constants/beaconConstants";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { SchoolSelect } from "@/components/common/SchoolSelect/SchoolSelect";
import { TableCard } from "@/components/common/TableCard/TableCard";
import "./BeaconsPage.css";

const { Search } = Input;
const { Option } = Select;

export const BeaconsPage = () => {
  const [search, setSearch] = useState("");
  const [type, setType] = useState(undefined);
  const [status, setStatus] = useState(undefined);
  const [districtId, setDistrictId] = useState(undefined);
  const [schoolId, setSchoolId] = useState(undefined);
  const [isAssigned, setIsAssigned] = useState(undefined);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const navigation = useAppNavigation();
  const { can } = usePermission();

  const params = {
    page,
    pageSize,
    search: search || undefined,
    type: type !== undefined ? type : undefined,
    status: status !== undefined ? status : undefined,
    districtId: districtId !== undefined ? districtId : undefined,
    schoolId: schoolId !== undefined ? schoolId : undefined,
    isAssigned: isAssigned !== undefined ? isAssigned : undefined,
  };

  const { data, isLoading, isFetching, error, refetch } = useBeacons(params);
  const deleteBeaconMutation = useDeleteBeacon();

  const handleView = (beacon) => {
    navigation.beacons.detail(beacon.id);
  };

  const handleCreate = () => {
    navigation.beacons.create();
  };

  const handleEdit = (beacon) => {
    navigation.beacons.edit(beacon.id);
  };

  const handleDelete = (beacon) => {
    deleteBeaconMutation.mutate(beacon.id);
  };

  const handlePageChange = (newPage, newPageSize) => {
    setPage(newPage);
    setPageSize(newPageSize);
  };

  const breadcrumbs = [{ label: "Beacons", path: "/beacons" }];

  return (
    <div>
      <PageHeader
        title="Beacons"
        breadcrumbs={breadcrumbs}
        extra={
          can(PERMISSIONS.BEACONS.CREATE) && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
              Create Beacon
            </Button>
          )
        }
      />

      <div className="toolbar">
        <Space wrap>
          <Search
            placeholder="Search beacons..."
            allowClear
            className="toolbar-search"
            onSearch={(value) => {
              setSearch(value);
              setPage(1);
            }}
            onChange={(e) => {
              if (!e.target.value) {
                setSearch("");
                setPage(1);
              }
            }}
          />
          <Select
            placeholder="Type"
            allowClear
            style={{ width: 130 }}
            value={type}
            onChange={(value) => {
              setType(value);
              setPage(1);
            }}
          >
            {Object.entries(BEACON_TYPES).map(([value, label]) => (
              <Option key={value} value={Number(value)}>
                {label}
              </Option>
            ))}
          </Select>
          <Select
            placeholder="Status"
            allowClear
            style={{ width: 140 }}
            value={status}
            onChange={(value) => {
              setStatus(value);
              setPage(1);
            }}
          >
            {Object.entries(BEACON_STATUSES).map(([value, label]) => (
              <Option key={value} value={Number(value)}>
                {label}
              </Option>
            ))}
          </Select>
          <DistrictSelect
            placeholder="All Districts"
            style={{ width: 180 }}
            value={districtId}
            onChange={(value) => {
              setDistrictId(value);
              setPage(1);
            }}
            allowClear
          />
          <SchoolSelect
            placeholder="All Schools"
            style={{ width: 180 }}
            value={schoolId}
            onChange={(value) => {
              setSchoolId(value);
              setPage(1);
            }}
            allowClear
          />
          <Space>
            <span>Assigned:</span>
            <Switch
              checked={isAssigned}
              onChange={(checked) => {
                setIsAssigned(checked);
                setPage(1);
              }}
            />
            {isAssigned !== undefined && (
              <Button
                type="link"
                size="small"
                onClick={() => {
                  setIsAssigned(undefined);
                  setPage(1);
                }}
              >
                Clear
              </Button>
            )}
          </Space>
          <Button
            icon={<ReloadOutlined />}
            onClick={() => refetch()}
            loading={isFetching}
            disabled={isFetching}
          >
            Refresh
          </Button>
        </Space>
      </div>

      <QueryStateHandler
        isLoading={isLoading}
        error={error}
        data={data}
        refetch={refetch}
        loadingDescription="Loading beacons..."
      >
        {(data) => {
          const items = data?.items || [];
          const paginationInfo = {
            page: data?.page || 1,
            pageSize: data?.pageSize || 10,
            totalCount: data?.totalCount || 0,
          };

          return (
            <TableCard>
              <BeaconTable
                data={items}
                loading={deleteBeaconMutation.isPending || isFetching}
                pagination={paginationInfo}
                onPageChange={handlePageChange}
                onView={handleView}
                onEdit={can(PERMISSIONS.BEACONS.UPDATE) ? handleEdit : null}
                onDelete={can(PERMISSIONS.BEACONS.DELETE) ? handleDelete : null}
              />
            </TableCard>
          );
        }}
      </QueryStateHandler>
    </div>
  );
};
