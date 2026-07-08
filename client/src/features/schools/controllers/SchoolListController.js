import { useState, useEffect } from "react";
import { withController } from "@/reactive/withController";
import { SchoolTable } from "../components/SchoolTable";
import services from "@/services";

const SchoolListController = ({ navigate }) => {
  const [schools, setSchools] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    page: 0,
    pageSize: 10,
    total: 0,
  });
  const [filters, setFilters] = useState({ isActive: true });
};
