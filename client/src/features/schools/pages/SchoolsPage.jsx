import { useState } from "react";
import { Input, Button, Space, Select } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { TableCard } from "@/components/common/TableCard/TableCard";
import { SchoolTable } from "../components/SchoolTable";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";
import { useSchools, useDeleteSchool } from "../hooks/useSchools";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { SCHOOL_LEVELS, SCHOOL_TYPES } from "../constants/schoolConstants";
import "./SchoolsPage.css";

const { Search } = Input;

export const SchoolsPage = () => {
  const [search, setSearch] = useState("");
  const [level, setLevel] = useState(undefined);
  const [type, setType] = useState(undefined);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const navigation = useAppNavigation();
  const { can } = usePermission();

  const params = {
    page,
    pageSize,
    search: search || undefined,
    level: level !== undefined ? level : undefined,
    type: type !== undefined ? type : undefined,
  };

  const { data, isLoading, isFetching, error, refetch } = useSchools(params);
  const deleteSchoolMutation = useDeleteSchool();

  const handleView = (school) => navigation.schools.detail(school.id);
  const handleCreate = () => navigation.schools.create();
  const handleEdit = (school) => navigation.schools.edit(school.id);
  const handleDelete = (school) => deleteSchoolMutation.mutate(school.id);
  const handlePageChange = (newPage, newPageSize) => {
    setPage(newPage);
    setPageSize(newPageSize);
  };

  const breadcrumbs = [{ label: "Schools", path: "/schools" }];

  return (
    <div>
      <PageHeader
        title="Schools"
        breadcrumbs={breadcrumbs}
        extra={
          can(PERMISSIONS.SCHOOLS.CREATE) && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
              Create School
            </Button>
          )
        }
      />

      <div className="toolbar">
        <Space wrap>
          <Search
            placeholder="Search schools..."
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
            placeholder="Level"
            allowClear
            style={{ width: 150 }}
            options={Object.entries(SCHOOL_LEVELS).map(([value, label]) => ({
              value: Number(value),
              label,
            }))}
            onChange={(value) => {
              setLevel(value);
              setPage(1);
            }}
          />
          <Select
            placeholder="Type"
            allowClear
            style={{ width: 150 }}
            options={Object.entries(SCHOOL_TYPES).map(([value, label]) => ({
              value: Number(value),
              label,
            }))}
            onChange={(value) => {
              setType(value);
              setPage(1);
            }}
          />
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
        loadingDescription="Loading schools..."
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
              <SchoolTable
                data={items}
                loading={deleteSchoolMutation.isPending || isFetching}
                pagination={paginationInfo}
                onPageChange={handlePageChange}
                onView={handleView}
                onEdit={can(PERMISSIONS.SCHOOLS.UPDATE) ? handleEdit : null}
                onDelete={can(PERMISSIONS.SCHOOLS.DELETE) ? handleDelete : null}
              />
            </TableCard>
          );
        }}
      </QueryStateHandler>
    </div>
  );
};
