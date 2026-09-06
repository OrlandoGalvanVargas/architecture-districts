import { theme as antdTheme } from "antd";
import { lightTokens, darkTokens } from "./tokens";

export const CSS_VAR_KEY = "facilityos";

export function buildThemeConfig(mode) {
  const isDark = mode === "dark";
  const base = isDark ? darkTokens : lightTokens;

  return {
    cssVar: { key: CSS_VAR_KEY },
    hashed: false,
    algorithm: isDark ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm,
    token: base.token,
    components: base.components,
  };
}
