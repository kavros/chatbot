import React, { useState, useRef, useEffect } from "react";
import "./Chatbot.css";
import apiClient from "../services/apiClient";
import Navbar from "../navbar/Navbar";

export interface Message {
  id: string;
  text: string;
  sender: "user" | "bot";
  timestamp: Date;
}

interface ChatbotProps {
  onSendMessage?: (message: string) => Promise<string>;
}

const Chatbot: React.FC<ChatbotProps> = ({ onSendMessage }) => {
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

    try {
      let botResponse;

      // Use apiClient instead of fetch
      const response = await apiClient("Agent/chat", {
        method: "POST",
        body: JSON.stringify({
          message: messageText.trim(),
          chatHistory: messages,
        }),
      });

      if (response) {
        botResponse = response.text;
      } else {
        botResponse = "Sorry, I couldn't get a response from the server.";
      }

      const botMessage: Message = {
        id: Date.now().toString() + "_bot",
        text: botResponse,
        sender: "bot",
        timestamp: new Date(),
      };

      setMessages((prev) => [...prev, botMessage]);
    } catch (error) {
      const errorMessage: Message = {
        id: Date.now().toString() + "_error",
        text: "Sorry, I encountered an error while processing your message. Please try again.",
        sender: "bot",
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, errorMessage]);
    } finally {
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
                  <div className="message-text">{message.text}</div>
                </div>
              </div>
            ))}

            {isTyping && (
              <div className="message-wrapper bot-message">
                <div className="message-avatar">
                  <div className="bot-avatar">🤖</div>
                </div>
                <div className="message-content">
                  <div className="typing-indicator">
                    <div className="typing-dot"></div>
                    <div className="typing-dot"></div>
                    <div className="typing-dot"></div>
                  </div>
                </div>
              </div>
            )}

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
