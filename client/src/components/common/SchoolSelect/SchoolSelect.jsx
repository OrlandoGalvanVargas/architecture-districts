import { Select } from "antd";
import { useSchools } from "@/features/schools/hooks/useSchools";

const { Option } = Select;

export const SchoolSelect = ({
  value,
  onChange,
  placeholder = "Select school",
  disabled = false,
  districtId = null,
  allowClear = true,
  ...restProps
}) => {
  const params = districtId ? { districtId, pageSize: 100 } : { pageSize: 100 };
  const { data, isLoading } = useSchools(params);

  const schools = data?.items || [];

  return (
    <Select
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      disabled={disabled || isLoading}
      loading={isLoading}
      showSearch
      optionFilterProp="label"
      allowClear={allowClear}
      {...restProps}
    >
      {schools.map((school) => (
        <Option key={school.id} value={school.id} label={school.name}>
          {school.name} ({school.schoolCode})
        </Option>
      ))}
    </Select>
  );
};
