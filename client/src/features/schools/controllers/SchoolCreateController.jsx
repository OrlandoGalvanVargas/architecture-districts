import { withController } from "@/reactive/withController";
import { Card } from "antd";
import { useEffect } from "react";
import { SchoolForm } from "../components/SchoolForm";
import { useNotification } from "@/contexts/Notification";
import { useAppNavigation } from "@/hooks/useAppNavigation";

export const SchoolCreateController = withController(
  ({ loading, actions, setCallback }) => {
    const createSchool = actions.createSchool;
    const isCreating = loading.createSchool;

    const navigate = useAppNavigation();
    const notification = useNotification();
  },
);
