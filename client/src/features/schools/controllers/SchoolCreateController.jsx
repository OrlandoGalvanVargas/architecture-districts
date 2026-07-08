import { useState } from "react";
import { withController } from "@/reactive/withController";
import { SchoolForm } from "../components/SchoolForm";
import services from "@/services";

const SchoolCreateController = ({ navigate }) => {
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      const result = await services.schools.create(values);
      navigate(`/schools/${result.id}`);
    } catch (error) {
      console.log(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <SchoolForm
      onSubmit={handleSubmit}
      onCancel={navigate("/schools")}
      loading={loading}
    />
  );
};

export default withController(SchoolCreateController);
