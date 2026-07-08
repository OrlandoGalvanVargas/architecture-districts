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

  const fetchSchools = async (params = {}) => {
    setLoading(true);
    try {
      const result = await services.schools.getAll({
        ...filters,
        ...params,
        page: params.Page || pagination.page,
        pageSize: pagination.pageSize,
      });
      setSchools(result.items);
      setPagination((prev) => ({
        ...prev,
        total: result.totalCount,
        page: result.page,
      }));
    } catch (error) {
      console.log(error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSchools();
  }, []);

  const handlePageChange = (page) => fetchSchools({ page });
  const handleFilterChange = (newFilters) => {
    setFilter(newFilters);
    fetchSchools({ ...newFilters, page: 1 });
  };

  return (
    <SchoolTable
      schools={schools}
      loading={loading}
      pagination={pagination}
      onPageChange={handlePageChange}
      onFilterChange={handleFilterChange}
      onView={(id) => navigate(`/schools/${id}`)}
      onCreate={() => navigate("/schools/create")}
    />
  );
};

export default withController(SchoolListController);
