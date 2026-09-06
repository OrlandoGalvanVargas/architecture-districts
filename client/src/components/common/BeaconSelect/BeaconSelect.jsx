import { useMemo } from "react";
import { Select } from "antd";
import { useBeacons, useBeacon } from "@/features/beacons/hooks/useBeacons";

const { Option } = Select;
const BEACON_QUERY_PARAMS = { status: 1, isAssigned: false, pageSize: 100 };

export const BeaconSelect = ({
  value,
  onChange,
  placeholder = "Select beacon (optional)",
  disabled = false,
  allowClear = true,
  ...restProps
}) => {
  const { data, isLoading } = useBeacons(BEACON_QUERY_PARAMS);
  const { data: currentBeacon } = useBeacon(value, { enabled: !!value });

  const options = useMemo(() => {
    const list = [...(data?.items || [])];
    if (currentBeacon && !list.some((b) => b.id === currentBeacon.id)) {
      list.unshift(currentBeacon);
    }
    return list;
  }, [data?.items, currentBeacon]);

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
      {options.map((beacon) => (
        <Option key={beacon.id} value={beacon.id} label={beacon.deviceName}>
          {beacon.deviceName} ({beacon.serialNumber})
        </Option>
      ))}
    </Select>
  );
};
