import React, { useState, useRef, useEffect } from "react";
import "./Chatbot.css";
import { streamApiClient } from "../services/apiClient";
import Navbar from "../navbar/Navbar";

export interface Message {
  id: string;
  text: string;
  sender: "user" | "bot";
  timestamp: Date;
}

const Chatbot: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputText, setInputText] = useState("");
  const [isTyping, setIsTyping] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  // Auto-scroll to bottom when new messages are added
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // Focus input on mount
  useEffect(() => {
    textareaRef.current?.focus();
  }, []);

  // Auto-resize textarea
  const adjustTextareaHeight = () => {
    const textarea = textareaRef.current;
    if (textarea) {
      textarea.style.height = "auto";
      textarea.style.height = Math.min(textarea.scrollHeight, 200) + "px";
    }
  };

  useEffect(() => {
    adjustTextareaHeight();
  }, [inputText]);

  const handleSendMessage = async (messageText: string) => {
    if (!messageText.trim()) return;

    const userMessage: Message = {
      id: Date.now().toString() + "_user",
      text: messageText.trim(),
      sender: "user",
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, userMessage]);
    setInputText("");
    setIsTyping(true);
    setIsLoading(true);

    // Create a placeholder bot message that will be updated as we stream
    const botMessageId = Date.now().toString() + "_bot";
    const botMessage: Message = {
      id: botMessageId,
      text: "",
      sender: "bot",
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, botMessage]);

    try {
      await streamApiClient(
        "Agent/chat/stream",
        {
          method: "POST",
          body: JSON.stringify({
            message: messageText.trim(),
            chatHistory: messages,
          }),
        },
        (data) => {
          // First chunk received - stop typing indicator but keep loading
          if (data.text && data.text.length > 0) {
            setIsTyping(false);
          }

          // Update the bot message with streaming text
          setMessages((prev) =>
            prev.map((msg) =>
              msg.id === botMessageId ? { ...msg, text: data.text } : msg
            )
          );

          // Stream completed
          if (data.isComplete) {
            setIsLoading(false);
          }
        },
        (error) => {
          console.error("Streaming error:", error);
          setIsTyping(false);
          setIsLoading(false);
          setMessages((prev) =>
            prev.map((msg) =>
              msg.id === botMessageId
                ? {
                    ...msg,
                    text: "Sorry, I encountered an error while processing your message. Please try again.",
                  }
                : msg
            )
          );
        },
        () => {
          // Stream completed
          setIsTyping(false);
          setIsLoading(false);
        }
      );
    } catch (error) {
      console.error("Error starting stream:", error);
      setMessages((prev) =>
        prev.map((msg) =>
          msg.id === botMessageId
            ? {
                ...msg,
                text: "Sorry, I encountered an error while processing your message. Please try again.",
              }
            : msg
        )
      );
      setIsTyping(false);
      setIsLoading(false);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    handleSendMessage(inputText);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage(inputText);
    }
  };

  const handleNewChat = () => {
    setMessages([]);
    setInputText("");
    setIsTyping(false);
    setIsLoading(false);
    textareaRef.current?.focus();
  };

  return (
    <div className="chatgpt-container">
      <Navbar />
      {/* Main chat area */}
      <div className="chatgpt-main">
        {messages.length === 0 ? (
          <div className="welcome-screen">
            <div className="welcome-content">
              <h1 className="welcome-title">How can I help you?</h1>
            </div>
          </div>
        ) : (
          <div className="messages-container">
            {messages.map((message) => (
              <div
                key={message.id}
                className={`message-wrapper ${message.sender}-message`}
              >
                <div className="message-avatar">
                  {message.sender === "user" ? (
                    <div className="user-avatar">👤</div>
                  ) : (
                    <div className="bot-avatar">🤖</div>
                  )}
                </div>
                <div className="message-content">
                  {message.sender === "bot" &&
                  message.text === "" &&
                  isTyping ? (
                    <div className="typing-indicator">
                      <div className="typing-dot"></div>
                      <div className="typing-dot"></div>
                      <div className="typing-dot"></div>
                    </div>
                  ) : (
                    <div className="message-text">{message.text}</div>
                  )}
                </div>
              </div>
            ))}

            <div ref={messagesEndRef} />
          </div>
        )}

        {/* Input area */}
        <div className="input-area">
          <div className="input-container">
            <form onSubmit={handleSubmit} className="input-form">
              <button
                type="button"
                className="input-action-btn"
                onClick={handleNewChat}
                title="Start new chat"
              >
                <span className="plus-icon">+</span>
              </button>
              <div className="input-wrapper">
                <textarea
                  ref={textareaRef}
                  value={inputText}
                  onChange={(e) => setInputText(e.target.value)}
                  onKeyDown={handleKeyDown}
                  placeholder="Ask anything..."
                  className="message-input"
                  disabled={isLoading}
                  rows={1}
                />
              </div>

              <button
                type="submit"
                className="send-btn-circle"
                disabled={!inputText.trim() || isLoading}
                title="Send message"
              >
                <span className="arrow-icon">↑</span>
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Chatbot;
