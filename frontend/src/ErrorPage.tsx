import React from 'react';
import './App.css';

const ErrorPage: React.FC = () => {
  return (
    <div className="centered-container">
      <div className="card">
        <h2>Access Denied</h2>
        <p>You must be logged in to view this page.</p>
      </div>
    </div>
  );
};

export default ErrorPage;
