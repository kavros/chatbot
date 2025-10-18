import { useNavigate } from "react-router-dom";
import "../App.css";
import apiClient from "../services/apiClient";
import { useLoading } from "../loading/LoadingContext";
import { useAuth } from "../AuthContext";
import { CredentialResponse, GoogleLogin } from "@react-oauth/google";

const Login: React.FC = () => {
  const { setLoading } = useLoading();
  const { setAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleSuccess = async (credentialResponse: CredentialResponse) => {
    try {
      setLoading(true);
      const token = credentialResponse.credential; // Extract the token

      // Send the token to the backend for verification
      const response = await apiClient("auth/login", {
        method: "POST",
        body: JSON.stringify({ token }),
      });

      if (response) {
        setAuthenticated(true); // Update global authentication state
        navigate("/main");
      } else {
        console.error("Login failed");
      }
    } catch (error) {
      console.error("Error verifying user:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="centered-container"
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        height: "100vh",
      }}
    >
      <h2 className="login-title">Login with Google</h2>
      <GoogleLogin onSuccess={handleSuccess} />
    </div>
  );
};

export default Login;
