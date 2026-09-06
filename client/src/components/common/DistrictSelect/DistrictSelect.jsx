import { Select } from "antd";
import { useDistricts } from "@/features/districts/hooks/useDistricts";

const { Option } = Select;

export const DistrictSelect = ({
  value,
  onChange,
  placeholder = "Select district",
  disabled = false,
  loading: externalLoading = false,
  allowClear = true,
  ...restProps
}) => {
  const { data: districts = [], isLoading: districtsLoading } = useDistricts();

  return (
    <Select
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      disabled={disabled || externalLoading || districtsLoading}
      loading={districtsLoading}
      showSearch
      optionFilterProp="label"
      allowClear={allowClear}
      {...restProps}
    >
      {districts.map((district) => (
        <Option key={district.id} value={district.id} label={district.name}>
          {district.name} ({district.code})
        </Option>
      ))}
    </Select>
  );
};
