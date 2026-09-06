import { Routes, Route, Navigate } from "react-router-dom";
import { UsersPage } from "./pages/UsersPage";
import { UserCreatePage } from "./pages/UserCreatePage";
import { UserDetailPage } from "./pages/UserDetailPage";
import { UserEditPage } from "./pages/UserEditPage";
import { ROUTES } from "@/router/routes.config";

export const UserView = () => {
  return (
    <Routes>
      <Route index element={<UsersPage />} />
      <Route path="create" element={<UserCreatePage />} />
      <Route path=":id" element={<UserDetailPage />} />
      <Route path=":id/edit" element={<UserEditPage />} />
      <Route path="*" element={<Navigate to={ROUTES.USERS.LIST} replace />} />
    </Routes>
  );
};
