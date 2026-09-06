/* eslint-disable no-console */
const isDev = import.meta.env.VITE_APP_ENV === "development";
const isLoggingEnabled = import.meta.env.VITE_ENABLE_LOGGING === "true";

class Logger {
  constructor() {
    this.enabled = isLoggingEnabled || isDev;
  }

  _formatMessage(level, message, ...args) {
    const timestamp = new Date().toISOString();
    return [`[${timestamp}] [${level.toUpperCase()}] ${message}`, ...args];
  }

  info(message, ...args) {
    if (!this.enabled) return;

    console.info(...this._formatMessage("info", message, ...args));
  }

  warn(message, ...args) {
    if (!this.enabled) return;
    console.warn(...this._formatMessage("warn", message, ...args));
  }

  error(message, ...args) {
    if (!this.enabled) return;
    console.error(...this._formatMessage("error", message, ...args));
  }

  debug(message, ...args) {
    if (!this.enabled || !isDev) return;

    console.debug(...this._formatMessage("debug", message, ...args));
  }

  track(event, data = {}) {
    if (!this.enabled) return;
    this.info(`[TRACK] ${event}`, data);
  }
}

export const logger = new Logger();
