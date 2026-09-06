import { palette } from "./palette";

const fontFamily =
  "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif";

export const lightTokens = {
  token: {
    colorPrimary: palette.brandCyan,
    colorInfo: palette.brandCyan,
    colorSuccess: palette.success,
    colorWarning: palette.warning,
    colorError: palette.error,

    colorBgLayout: palette.neutral.gray50,
    colorBgContainer: palette.neutral.white,
    colorBgElevated: palette.neutral.white,
    colorBorder: palette.neutral.gray200,
    colorBorderSecondary: palette.neutral.gray100,
    colorText: palette.neutral.gray800,
    colorTextSecondary: palette.neutral.gray600,
    colorTextTertiary: palette.neutral.gray500,

    borderRadius: 10,
    borderRadiusLG: 14,
    fontFamily,
    fontSize: 14,
    controlHeight: 38,

    boxShadowSecondary: "0 6px 16px rgba(11, 59, 96, 0.08)",
  },
  components: {
    Layout: {
      headerBg: palette.brandNavy,
      headerColor: palette.neutral.white,
      siderBg: palette.brandNavy,
      bodyBg: palette.neutral.gray50,
      headerHeight: 64,
      headerPadding: "0 24px",
    },
    Menu: {
      darkItemBg: "transparent",
      darkItemColor: "rgba(255,255,255,0.75)",
      darkItemHoverBg: "rgba(255,255,255,0.08)",
      darkItemSelectedBg: palette.brandCyan,
      darkItemSelectedColor: palette.neutral.white,
      darkSubMenuItemBg: palette.brandNavyDeep,
      itemBorderRadius: 8,
    },
    Button: {
      controlHeight: 38,
      fontWeight: 500,
      primaryShadow: "0 2px 6px rgba(0, 168, 204, 0.28)",
    },
    Card: {
      borderRadiusLG: 14,
      boxShadowTertiary: "0 1px 2px rgba(11, 59, 96, 0.04), 0 4px 12px rgba(11, 59, 96, 0.06)",
      headerFontSize: 16,
    },
    Table: {
      headerBg: palette.neutral.gray50,
      headerColor: palette.neutral.gray700,
      rowHoverBg: palette.neutral.gray50,
      borderColor: palette.neutral.gray200,
      headerBorderRadius: 10,
    },
    Input: { controlHeight: 38, activeBorderColor: palette.brandCyan },
    Select: { controlHeight: 38 },
  },
};

export const darkTokens = {
  token: {
    colorPrimary: palette.brandCyan,
    colorInfo: palette.brandCyan,
    colorSuccess: palette.success,
    colorWarning: palette.warning,
    colorError: palette.error,

    colorBgLayout: palette.neutral.gray900,
    colorBgContainer: palette.neutral.gray800,
    colorBgElevated: palette.neutral.gray800,
    colorBorder: palette.neutral.gray700,
    colorBorderSecondary: "#2A323C",
    colorText: palette.neutral.gray100,
    colorTextSecondary: palette.neutral.gray400,
    colorTextTertiary: palette.neutral.gray500,

    borderRadius: 10,
    borderRadiusLG: 14,
    fontFamily,
    fontSize: 14,
    controlHeight: 38,

    boxShadowSecondary: "0 6px 16px rgba(0, 0, 0, 0.35)",
  },
  components: {
    Layout: {
      headerBg: palette.brandNavyDeep,
      headerColor: palette.neutral.white,
      siderBg: palette.brandNavyDeep,
      bodyBg: palette.neutral.gray900,
      headerHeight: 64,
      headerPadding: "0 24px",
    },
    Menu: {
      darkItemBg: "transparent",
      darkItemColor: "rgba(255,255,255,0.7)",
      darkItemHoverBg: "rgba(255,255,255,0.06)",
      darkItemSelectedBg: palette.brandCyan,
      darkItemSelectedColor: palette.neutral.white,
      darkSubMenuItemBg: "#071E33",
      itemBorderRadius: 8,
    },
    Button: {
      controlHeight: 38,
      fontWeight: 500,
      primaryShadow: "0 2px 6px rgba(0, 168, 204, 0.35)",
    },
    Card: {
      borderRadiusLG: 14,
      boxShadowTertiary: "0 1px 2px rgba(0,0,0,0.3), 0 4px 14px rgba(0,0,0,0.35)",
      headerFontSize: 16,
    },
    Table: {
      headerBg: "#1C232B",
      headerColor: palette.neutral.gray300,
      rowHoverBg: "#20282F",
      borderColor: palette.neutral.gray700,
      headerBorderRadius: 10,
    },
    Input: { controlHeight: 38, activeBorderColor: palette.brandCyan },
    Select: { controlHeight: 38 },
  },
};
