import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Chatbot from "./Chatbot";
import apiClient from "../services/apiClient";

jest.mock("../services/apiClient");
jest.mock("../navbar/Navbar", () => {
  return function MockNavbar() {
    return <div data-testid="navbar">Mock Navbar</div>;
  };
});

const mockApiClient = apiClient as jest.MockedFunction<typeof apiClient>;

describe("Chatbot Component", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    Element.prototype.scrollIntoView = jest.fn();
  });

  it("should render welcome screen and send a user message", async () => {
    mockApiClient.mockResolvedValue({ text: "Hello user!" });
    render(<Chatbot />);

    expect(screen.getByText("How can I help you?")).toBeInTheDocument();

    const textarea = screen.getByPlaceholderText("Ask anything...");
    const sendButton = screen.getByTitle("Send message");

    userEvent.type(textarea, "Hello bot");
    userEvent.click(sendButton);

    await waitFor(() => {
      expect(screen.getByText("Hello bot")).toBeInTheDocument();
    });
  });

  it("should clear chat and reset to welcome screen", async () => {
    mockApiClient.mockResolvedValue({ text: "Bot response" });
    render(<Chatbot />);

    const textarea = screen.getByPlaceholderText("Ask anything...");
    const sendButton = screen.getByTitle("Send message");

    userEvent.type(textarea, "Hello bot");
    userEvent.click(sendButton);

    await waitFor(() => {
      expect(screen.getByText("Hello bot")).toBeInTheDocument();
    });

    const newChatButton = screen.getByTitle("Start new chat");
    userEvent.click(newChatButton);

    expect(screen.getByText("How can I help you?")).toBeInTheDocument();
    expect(screen.queryByText("Hello bot")).not.toBeInTheDocument();
  });
});
