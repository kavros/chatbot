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
      navigate("/login");
    } catch (error) {
      console.error("Error during logout:", error);
    }
  };

  return (
    <>
      <div className="chatgpt-header">
        <div className="header-left">
          <div className="chatgpt-title">Askly</div>
        </div>
        <div className="header-right">
          <button className="logout-btn" onClick={handleLogout} title="Logout">
            <span className="logout-icon">⬆</span>
            Logout
          </button>
        </div>
      </div>
    </>
  );
};

export default Navbar;
