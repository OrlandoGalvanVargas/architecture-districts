import { useState, useEffect } from "react";
import { withController } from "@/reactive/withController";
import { SchoolForm } from "../components/SchoolForm";
import services from "@/services";
import { useAppNavigation } from "@/hooks/useAppNavigation";

const SchoolEditController = ({ navigate, params }) => {
  const [school, setSchool] = useState(null);
  const [loading, setLoading] = useState(false);
  const navigation = useAppNavigation();

  useEffect(() => {
    const fetch = async () => {
      const result = await services.schools.getById(params.id);
      setSchool(result);
    };

    fetch();
  }, [params.id]);

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      await services.schools.update(params.id, values);
      navigate(`/schools/${params.id}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <SchoolForm
      initialValues={school}
      onSubmit={handleSubmit}
      onCancel={() => navigate(`/schools/${params.id}`)}
      loading={loading}
    />
  );
};

export default withController(SchoolEditController);
