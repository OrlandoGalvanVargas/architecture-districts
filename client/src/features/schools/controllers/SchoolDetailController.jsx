import { useState, useEffect } from "react";
import { withController } from "@/reactive/withController";
import { SchoolDetail } from "../components/SchoolDetail";
import services from "@/services";

const SchoolDetailController = ({ navigate, params }) => {
  const [school, setSchool] = useState(null);
  const [loading, setLoading] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    const fetch = async () => {
      setLoading(true);
      try {
        const result = await services.schools.getById(params.id);
        setSchool(result);
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, [params.id]);

  const handleDelete = async () => {
    setIsDeleting(true);
    try {
      await services.schools.delete(params.id);
      navigate("/schools");
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <SchoolDetail
      school={school}
      loading={loading}
      isDeleting={isDeleting}
      onEdit={() => navigate(`/schools/${params.id}/edit`)}
      onDelete={handleDelete}
      onBack={() => navigate("/schools")}
    />
  );
};

export default withController(SchoolDetailController);
