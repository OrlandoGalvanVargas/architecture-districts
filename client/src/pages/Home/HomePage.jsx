import { useMemo } from "react";
import { Typography, Tag, List, theme as antdTheme } from "antd";
import {
  BankOutlined,
  BookOutlined,
  TeamOutlined,
  ApiOutlined,
  UserOutlined,
  RightOutlined,
  CheckCircleFilled,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { usePermission } from "@/hooks/usePermission";
import { useAppNavigation } from "@/hooks/useAppNavigation";
import { PERMISSIONS } from "@/utils/permissions";
import { ROUTES } from "@/router/routes.config";
import { useDistricts } from "@/features/districts/hooks/useDistricts";
import { useSchools } from "@/features/schools/hooks/useSchools";
import { useBeacons } from "@/features/beacons/hooks/useBeacons";
import { useFaculties } from "@/features/faculties/hooks/useFaculties";
import { useUsers } from "@/features/users/hooks/useUsers";
import "./HomePage.css";

const { Title, Text } = Typography;

export const HomePage = () => {
  const navigate = useNavigate();
  const navigation = useAppNavigation();
  const { user } = useAuth();
  const { can } = usePermission();
  const { token } = antdTheme.useToken();

  const { data: districtsData } = useDistricts();
  const { data: schoolsData } = useSchools({ page: 1, pageSize: 1 });
  const { data: facultiesData } = useFaculties({ page: 1, pageSize: 1 });
  const { data: usersData } = useUsers({ page: 1, pageSize: 1 });
  const { data: beaconsData } = useBeacons({ page: 1, pageSize: 1 });
  const { data: unassignedData } = useBeacons({ page: 1, pageSize: 5, isAssigned: false });

  const modules = useMemo(
    () => [
      {
        key: "districts",
        title: "Districts",
        description: "Districts and their assigned schools",
        icon: <BankOutlined />,
        route: ROUTES.DISTRICTS.LIST,
        permission: PERMISSIONS.DISTRICTS.VIEW_LIST,
        count: districtsData?.length,
      },
      {
        key: "schools",
        title: "Schools",
        description: "Campuses, capacity and district assignment",
        icon: <BookOutlined />,
        route: ROUTES.SCHOOLS.LIST,
        permission: PERMISSIONS.SCHOOLS.VIEW_LIST,
        count: schoolsData?.totalCount,
      },
      {
        key: "faculties",
        title: "Faculty",
        description: "Staff records and beacon assignments",
        icon: <TeamOutlined />,
        route: ROUTES.FACULTIES.LIST,
        permission: PERMISSIONS.FACULTIES.VIEW_LIST,
        count: facultiesData?.totalCount,
      },
      {
        key: "beacons",
        title: "Beacons",
        description: "Device inventory and assignment status",
        icon: <ApiOutlined />,
        route: ROUTES.BEACONS.LIST,
        permission: PERMISSIONS.BEACONS.VIEW_LIST,
        count: beaconsData?.totalCount,
      },
      {
        key: "users",
        title: "Users",
        description: "Administrative accounts and roles",
        icon: <UserOutlined />,
        route: ROUTES.USERS.LIST,
        permission: PERMISSIONS.USERS.VIEW_LIST,
        count: usersData?.totalCount,
      },
    ],
    [districtsData, schoolsData, facultiesData, beaconsData, usersData]
  );

  const accessibleModules = modules.filter((m) => can(m.permission));
  const attentionItems = unassignedData?.items || [];

  const today = new Date().toLocaleDateString("en-US", {
    weekday: "long",
    month: "long",
    day: "numeric",
  });

  return (
    <div className="home-page">
      <div className="home-topbar">
        <div>
          <Text className="home-eyebrow">{today}</Text>
          <Title level={3} className="home-title">
            {user?.name?.split(" ")[0] || "Overview"}
          </Title>
        </div>
      </div>

      <div
        className="home-stats-bar"
        style={{
          backgroundColor: token.colorBgContainer,
          borderColor: token.colorBorderSecondary,
          borderRadius: token.borderRadiusLG,
        }}
      >
        {accessibleModules.map((m) => (
          <div
            className="home-stat"
            key={m.key}
            style={{ borderColor: token.colorBorderSecondary }}
          >
            <span className="home-stat-value">{m.count ?? "—"}</span>
            <span className="home-stat-label">{m.title}</span>
          </div>
        ))}
      </div>

      <div className="home-grid">
        <div>
          <div className="home-section-label">Modules</div>
          <div
            className="home-module-list"
            style={{
              backgroundColor: token.colorBgContainer,
              borderColor: token.colorBorderSecondary,
              borderRadius: token.borderRadiusLG,
            }}
          >
            {accessibleModules.map((m) => (
              <div
                key={m.key}
                className="home-module-row"
                style={{ borderColor: token.colorBorderSecondary }}
                onClick={() => navigate(m.route)}
                role="button"
                tabIndex={0}
              >
                <div
                  className="home-module-icon"
                  style={{ backgroundColor: token.colorPrimaryBg, color: token.colorPrimary }}
                >
                  {m.icon}
                </div>
                <div className="home-module-text">
                  <span className="home-module-title">{m.title}</span>
                  <span className="home-module-desc">{m.description}</span>
                </div>
                <RightOutlined className="home-module-arrow" />
              </div>
            ))}
          </div>
        </div>

        {can(PERMISSIONS.BEACONS.VIEW_LIST) && (
          <div>
            <div className="home-section-label">Needs attention</div>
            <div
              className="home-attention-panel"
              style={{
                backgroundColor: token.colorBgContainer,
                borderColor: token.colorBorderSecondary,
                borderRadius: token.borderRadiusLG,
              }}
            >
              {attentionItems.length === 0 ? (
                <div className="home-attention-empty">
                  <CheckCircleFilled style={{ color: token.colorSuccess }} />
                  <span>All beacons are assigned</span>
                </div>
              ) : (
                <List
                  size="small"
                  dataSource={attentionItems}
                  renderItem={(beacon) => (
                    <List.Item
                      className="home-attention-item"
                      style={{ borderColor: token.colorBorderSecondary }}
                      onClick={() => navigation.beacons.detail(beacon.id)}
                    >
                      <span className="home-attention-name">{beacon.deviceName}</span>
                      <Tag color="orange">Unassigned</Tag>
                    </List.Item>
                  )}
                />
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
