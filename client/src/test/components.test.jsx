import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { ConfigProvider } from "antd";
import { ConfirmDialog } from "@/components/common/ConfirmDialog/ConfirmDialog";
import { EmptyState } from "@/components/common/EmptyState/EmptyState";
import { ErrorMessage } from "@/components/common/ErrorMessage/ErrorMessage";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";

const renderAnt = (ui) => render(<ConfigProvider>{ui}</ConfigProvider>);

describe("ErrorMessage", () => {
  it("renders string and structured errors with retry", () => {
    const onRetry = vi.fn();

    renderAnt(
      <ErrorMessage
        error={{ status: 404, friendlyMessage: "District missing" }}
        onRetry={onRetry}
      />
    );

    expect(screen.getByText("Error 404")).toBeInTheDocument();
    expect(screen.getByText("District missing")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(onRetry).toHaveBeenCalledOnce();
  });

  it("reveals and hides technical details when requested", () => {
    renderAnt(
      <ErrorMessage
        error={{ message: "Request failed", details: { traceId: "abc" } }}
        showDetails
      />
    );

    expect(screen.queryByText(/traceId/)).not.toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Show Details" }));
    expect(screen.getByText(/"traceId": "abc"/)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Hide Details" }));
    expect(screen.queryByText(/traceId/)).not.toBeInTheDocument();
  });

  it("renders nothing without an error", () => {
    const { container } = renderAnt(<ErrorMessage error={null} />);

    expect(container).toBeEmptyDOMElement();
  });
});

describe("ConfirmDialog", () => {
  it("calls the confirm and cancel actions with custom labels", () => {
    const onConfirm = vi.fn();
    const onCancel = vi.fn();

    renderAnt(
      <ConfirmDialog
        open
        title="Delete district"
        description="This cannot be undone."
        confirmText="Delete"
        cancelText="Keep"
        onConfirm={onConfirm}
        onCancel={onCancel}
      />
    );

    expect(screen.getByText("Delete district")).toBeInTheDocument();
    expect(screen.getByText("This cannot be undone.")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Delete" }));
    fireEvent.click(screen.getByRole("button", { name: "Keep" }));
    expect(onConfirm).toHaveBeenCalledOnce();
    expect(onCancel).toHaveBeenCalledOnce();
  });
});

describe("EmptyState and LoadingSpinner", () => {
  it("renders an optional action only when provided", () => {
    const onAction = vi.fn();

    renderAnt(
      <EmptyState
        title="No districts"
        description="Create the first district."
        actionText="Create district"
        onAction={onAction}
      />
    );

    fireEvent.click(screen.getByRole("button", { name: "Create district" }));
    expect(onAction).toHaveBeenCalledOnce();
  });

  it("renders loading descriptions and presentation modes", () => {
    const { rerender } = renderAnt(<LoadingSpinner description="Loading districts" />);
    expect(screen.getByText("Loading districts")).toBeInTheDocument();

    rerender(
      <ConfigProvider>
        <LoadingSpinner fullScreen description="Loading app" />
      </ConfigProvider>
    );
    expect(screen.getByText("Loading app")).toBeInTheDocument();
    expect(document.querySelector(".loading-spinner--fullscreen")).toBeInTheDocument();
  });
});
