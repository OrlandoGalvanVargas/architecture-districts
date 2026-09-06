import { useState } from "react";
import { Input, Button, Space, Select, InputNumber, Switch } from "antd";
import { PlusOutlined, ReloadOutlined } from "@ant-design/icons";
import { PageHeader } from "@/components/Layout/PageHeader/PageHeader";
import { UserTable } from "../components/UserTable";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";
import { useUsers, useDeleteUser } from "../hooks/useUsers";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { usePermission } from "@/hooks/usePermission";
import { PERMISSIONS } from "@/utils/permissions";
import { USER_ROLES, ENTITY_TYPES } from "../constants/userConstants";
import { TableCard } from "@/components/common/TableCard/TableCard";
import "./UsersPage.css";

const { Search } = Input;
const { Option } = Select;

export const UsersPage = () => {
  const [search, setSearch] = useState("");
  const [role, setRole] = useState(undefined);
  const [entityType, setEntityType] = useState(undefined);
  const [entityId, setEntityId] = useState(undefined);
  const [isActive, setIsActive] = useState(undefined);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const navigation = useAppNavigation();
  const { can } = usePermission();

  const params = {
    page,
    pageSize,
    search: search || undefined,
    role: role || undefined,
    entityType: entityType !== undefined ? entityType : undefined,
    entityId: entityId !== undefined ? entityId : undefined,
    isActive: isActive !== undefined ? isActive : undefined,
  };

  const { data, isLoading, isFetching, error, refetch } = useUsers(params);
  const deleteUserMutation = useDeleteUser();

  const handleView = (user) => {
    navigation.users.detail(user.id);
  };

  const handleCreate = () => {
    navigation.users.create();
  };

  const handleEdit = (user) => {
    navigation.users.edit(user.id);
  };

  const handleDelete = (user) => {
    deleteUserMutation.mutate(user.id);
  };

  const handlePageChange = (newPage, newPageSize) => {
    setPage(newPage);
    setPageSize(newPageSize);
  };

  const breadcrumbs = [{ label: "Users", path: "/users" }];

  return (
    <div>
      <PageHeader
        title="Users"
        breadcrumbs={breadcrumbs}
        extra={
          can(PERMISSIONS.USERS.CREATE) && (
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
              Create User
            </Button>
          )
        }
      />

      <div className="toolbar">
        <Space wrap>
          <Search
            placeholder="Search users..."
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
            placeholder="Role"
            allowClear
            style={{ width: 150 }}
            value={role}
            onChange={(value) => {
              setRole(value);
              setPage(1);
            }}
          >
            {Object.values(USER_ROLES).map((r) => (
              <Option key={r} value={r}>
                {r}
              </Option>
            ))}
          </Select>
          <Select
            placeholder="Entity Type"
            allowClear
            style={{ width: 150 }}
            value={entityType}
            onChange={(value) => {
              setEntityType(value);
              setPage(1);
            }}
          >
            {Object.entries(ENTITY_TYPES).map(([value, label]) => (
              <Option key={value} value={Number(value)}>
                {label}
              </Option>
            ))}
          </Select>
          <InputNumber
            placeholder="Entity ID"
            style={{ width: 120 }}
            min={1}
            value={entityId}
            onChange={(value) => {
              setEntityId(value);
              setPage(1);
            }}
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
        loadingDescription="Loading users..."
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
              <UserTable
                data={items}
                loading={deleteUserMutation.isPending || isFetching}
                pagination={paginationInfo}
                onPageChange={handlePageChange}
                onView={handleView}
                onEdit={can(PERMISSIONS.USERS.UPDATE) ? handleEdit : null}
                onDelete={can(PERMISSIONS.USERS.DELETE) ? handleDelete : null}
              />
            </TableCard>
          );
        }}
      </QueryStateHandler>
    </div>
  );
};
