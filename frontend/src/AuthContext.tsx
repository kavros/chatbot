import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import apiClient from "./services/apiClient";

interface AuthContextProps {
  isAuthenticated: boolean;
  loading: boolean;
  setAuthenticated: (value: boolean) => void;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [isAuthenticated, setAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true); // New loading state

  useEffect(() => {
    const validateAuth = async () => {
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

  return (
    <AuthContext.Provider
      value={{ isAuthenticated, loading, setAuthenticated }}
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
