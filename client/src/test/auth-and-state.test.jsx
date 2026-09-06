import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { ConfigProvider } from "antd";
import { LoginForm } from "@/features/auth/components/LoginForm";
import { QueryStateHandler } from "@/components/common/QueryStateHandler/QueryStateHandler";

const renderAnt = (ui) => render(<ConfigProvider>{ui}</ConfigProvider>);

describe("LoginForm", () => {
  it("blocks invalid credentials before submitting", async () => {
    const onSubmit = vi.fn();
    const user = userEvent.setup();

    renderAnt(<LoginForm onSubmit={onSubmit} />);
    await user.click(screen.getByRole("button", { name: "Sign In" }));

    expect(await screen.findByText("Please enter your email")).toBeInTheDocument();
    expect(screen.getByText("Please enter your password")).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("submits valid credentials and reports failed authentication", async () => {
    const onSubmit = vi.fn().mockResolvedValue({ success: false, error: "Invalid credentials" });
    const user = userEvent.setup();

    renderAnt(<LoginForm onSubmit={onSubmit} />);
    await user.type(screen.getByPlaceholderText("you@school.edu"), "user@example.com");
    await user.type(screen.getByPlaceholderText("Enter your password"), "secret123");
    await user.click(screen.getByRole("button", { name: "Sign In" }));

    await waitFor(() =>
      expect(onSubmit).toHaveBeenCalledWith({
        email: "user@example.com",
        password: "secret123",
      })
    );
    expect(await screen.findByText("Invalid credentials")).toBeInTheDocument();
  });

  it("keeps the submit button loading after a successful request for parent navigation", async () => {
    const onSubmit = vi.fn().mockResolvedValue({ success: true });
    const user = userEvent.setup();

    const { container } = renderAnt(<LoginForm onSubmit={onSubmit} />);
    await user.type(screen.getByPlaceholderText("you@school.edu"), "user@example.com");
    await user.type(screen.getByPlaceholderText("Enter your password"), "secret123");
    await user.click(screen.getByRole("button", { name: "Sign In" }));

    const submitButton = container.querySelector('button[type="submit"]');
    await waitFor(() => expect(onSubmit).toHaveBeenCalled());
    expect(submitButton).toHaveClass("ant-btn-loading");
  });
});

describe("QueryStateHandler", () => {
  it("renders loading and error states without invoking children", () => {
    const children = vi.fn(() => <div>Loaded content</div>);
    const refetch = vi.fn();

    renderAnt(
      <QueryStateHandler
        isLoading
        error={null}
        data={null}
        refetch={refetch}
        loadingDescription="Loading data"
      >
        {children}
      </QueryStateHandler>
    );
    expect(screen.getByText("Loading data")).toBeInTheDocument();
    expect(children).not.toHaveBeenCalled();

    renderAnt(
      <QueryStateHandler
        isLoading={false}
        error={{ friendlyMessage: "Unable to load" }}
        data={null}
        refetch={refetch}
      >
        {children}
      </QueryStateHandler>
    );
    expect(screen.getByText("Unable to load")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /Retry/ }));
    expect(refetch).toHaveBeenCalledOnce();
  });

  it("keeps stale data visible and offers retry for refresh errors", () => {
    const refetch = vi.fn();

    renderAnt(
      <QueryStateHandler
        isLoading={false}
        error={{ message: "Refresh failed" }}
        data={[{ id: 1 }]}
        refetch={refetch}
      >
        {(data) => <div>Rows: {data.length}</div>}
      </QueryStateHandler>
    );

    expect(screen.getByText("Could not refresh data.")).toBeInTheDocument();
    expect(screen.getByText("Rows: 1")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /Retry/ }));
    expect(refetch).toHaveBeenCalledOnce();
  });
});
