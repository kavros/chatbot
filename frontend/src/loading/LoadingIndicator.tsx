import React from "react";
import { useLoading } from "./LoadingContext";
import "../App.css";

const LoadingIndicator: React.FC = () => {
  const { isLoading } = useLoading();

  if (!isLoading) return null;

  return (
    <div className="loading-container">
      <div className="spinner"></div>
    </div>
  );
};

export default LoadingIndicator;
