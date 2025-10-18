import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthContext";
import "./Navbar.css";

interface NavbarProps {
  children?: React.ReactNode;
}

const Navbar: React.FC<NavbarProps> = ({ children }) => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logout();
      navigate("/");
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <div className="navbar-brand">
          <h1>Askly</h1>
        </div>
        <div className="navbar-menu">
          {children}
          <button className="navbar-btn logout-btn" onClick={handleLogout}>
            <span className="icon" aria-hidden>
              ⎋
            </span>
            <span className="btn-text">Logout</span>
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
