import { useState } from "react";
import { Input, Button, Space, Switch } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { FacultyTable } from "../components/FacultyTable";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";
import { useFaculties, useDeleteFaculty } from "../hooks/useFaculties";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { DistrictSelect } from "@/components/common/DistrictSelect/DistrictSelect";
import { SchoolSelect } from "@/components/common/SchoolSelect/SchoolSelect";
import { TableCard } from "@/components/common/TableCard/TableCard";
import "./FacultiesPage.css";

const { Search } = Input;

export const FacultiesPage = () => {
  const [search, setSearch] = useState("");
  const [districtId, setDistrictId] = useState(undefined);
  const [schoolId, setSchoolId] = useState(undefined);
  const [isActive, setIsActive] = useState(undefined);
  const [hasBeacon, setHasBeacon] = useState(undefined);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const navigation = useAppNavigation();
  const { can } = usePermission();

  const params = {
    page,
    pageSize,
    search: search || undefined,
    districtId: districtId !== undefined ? districtId : undefined,
    schoolId: schoolId !== undefined ? schoolId : undefined,
    isActive: isActive !== undefined ? isActive : undefined,
    hasBeacon: hasBeacon !== undefined ? hasBeacon : undefined,
  };

  const { data, isLoading, isFetching, error, refetch } = useFaculties(params);
  const deleteFacultyMutation = useDeleteFaculty();

  const handleView = (faculty) => {
    navigation.faculties.detail(faculty.id);
  };

  const handleCreate = () => {
    navigation.faculties.create();
  };

  const handleEdit = (faculty) => {
    navigation.faculties.edit(faculty.id);
  };

  const handleDelete = (faculty) => {
    deleteFacultyMutation.mutate(faculty.id);
  };

  const handlePageChange = (newPage, newPageSize) => {
    setPage(newPage);
    setPageSize(newPageSize);
  };

  const breadcrumbs = [{ label: "Faculties", path: "/faculties" }];

  return (
    <div>
      <PageHeader
        title="Faculties"
        breadcrumbs={breadcrumbs}
        extra={
          can(PERMISSIONS.FACULTIES.CREATE) && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
              Create Faculty
            </Button>
          )
        }
      />

      <div className="toolbar">
        <Space wrap>
          <Search
            placeholder="Search faculties..."
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
            <span>Active:</span>
            <Switch
              checked={isActive}
              onChange={(checked) => {
                setIsActive(checked);
                setPage(1);
              }}
            />
            {isActive !== undefined && (
              <Button
                type="link"
                size="small"
                onClick={() => {
                  setIsActive(undefined);
                  setPage(1);
                }}
              >
                Clear
              </Button>
            )}
          </Space>
          <Space>
            <span>Has Beacon:</span>
            <Switch
              checked={hasBeacon}
              onChange={(checked) => {
                setHasBeacon(checked);
                setPage(1);
              }}
            />
            {hasBeacon !== undefined && (
              <Button
                type="link"
                size="small"
                onClick={() => {
                  setHasBeacon(undefined);
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
        loadingDescription="Loading faculties..."
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
              <FacultyTable
                data={items}
                loading={deleteFacultyMutation.isPending || isFetching}
                pagination={paginationInfo}
                onPageChange={handlePageChange}
                onView={handleView}
                onEdit={can(PERMISSIONS.FACULTIES.UPDATE) ? handleEdit : null}
                onDelete={can(PERMISSIONS.FACULTIES.DELETE) ? handleDelete : null}
              />
            </TableCard>
          );
        }}
      </QueryStateHandler>
    </div>
  );
};
