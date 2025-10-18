import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import apiClient from "./services/apiClient";
import { PUBLIC_ROUTES_ARRAY } from "./constants/routes";

interface AuthContextProps {
  isAuthenticated: boolean;
  loading: boolean;
  setAuthenticated: (value: boolean) => void;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [isAuthenticated, setAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true); // New loading state

  useEffect(() => {
    const validateAuth = async () => {
      // Skip validation on public routes
      if (PUBLIC_ROUTES_ARRAY.includes(window.location.pathname)) {
        setLoading(false);
        return;
      }

      try {
        const response = await apiClient("auth/validate"); // Call backend endpoint
        if (response.isAuthenticated) {
          setAuthenticated(true);
        }
      } catch (error) {
        console.error("Error validating authentication:", error);
        setAuthenticated(false);
      } finally {
        setLoading(false); // Validation complete
      }
    };

    validateAuth();
  }, []);

  const logout = async () => {
    try {
      await apiClient("auth/logout", {
        method: "POST",
      });
    } catch (error) {
      console.error("Error during logout:", error);
    } finally {
      setAuthenticated(false);
    }
  };

  return (
    <AuthContext.Provider
      value={{ isAuthenticated, loading, setAuthenticated, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
