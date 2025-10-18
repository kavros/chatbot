import React from "react";
import { Link } from "react-router-dom";
import "./App.css";

const ErrorPage: React.FC = () => {
  return (
    <div className="centered-container">
      <div className="card">
        <h2>Access Denied</h2>
        <p>You must be logged in to view this page.</p>
        <Link to="/" className="login-link">
          Go to Login Page
        </Link>
      </div>
    </div>
  );
};

export default ErrorPage;
