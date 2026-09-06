export const themeConfig = {
  token: {
    colorPrimary: "#1677ff",
    colorSuccess: "#52c41a",
    colorWarning: "#faad14",
    colorError: "#ff4d4f",
    colorInfo: "#1677ff",
    borderRadius: 8,
    fontSize: 14,
    fontFamily:
      "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif",

    colorBgContainer: "var(--color-surface, #ffffff)",
    colorText: "var(--color-text, #1f1f1f)",
    colorBorder: "var(--color-border, #e5e7eb)",
  },
  components: {
    Layout: {
      headerBg: "var(--header-bg, #001529)",
      siderBg: "var(--sider-bg, #001529)",
      bodyBg: "var(--body-bg, #f5f7fa)",
    },
    Menu: {
      darkItemBg: "transparent",
      darkItemSelectedBg: "var(--color-primary, #1677ff)",
      darkItemSelectedColor: "#ffffff",
    },
    Table: {
      headerBg: "var(--table-header-bg, #fafafa)",
      rowHoverBg: "var(--table-row-hover-bg, #f5f5f5)",
    },
    Button: {
      controlHeight: 36,
    },
    Card: {
      boxShadowTertiary: "0 1px 2px rgba(0, 0, 0, 0.03), 0 1px 6px -1px rgba(0, 0, 0, 0.02)",
    },
  },
};
