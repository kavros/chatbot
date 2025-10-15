import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "./App.css";
import Login from "./login/Login";
import ProtectedRoute from "./router/ProtectedRoute";
import ErrorPage from "./ErrorPage";
import { LoadingProvider } from "./loading/LoadingContext";
import LoadingIndicator from "./loading/LoadingIndicator";
import { AuthProvider } from "./AuthContext";
import { GoogleOAuthProvider } from "@react-oauth/google";
import Chatbot from "./chat/Chatbot";

function App() {
  return (
    <GoogleOAuthProvider
      clientId={process.env.REACT_APP_GOOGLE_AUTH_CLIENT_ID || ""}
    >
      <AuthProvider>
        <LoadingProvider>
          <LoadingIndicator />
          <Router>
            <Routes>
              <Route path="/" element={<Login />} />
              <Route path="/error" element={<ErrorPage />} />
              <Route
                path="/main"
                element={
                  <ProtectedRoute>
                    <Chatbot />
                  </ProtectedRoute>
                }
              />
            </Routes>
          </Router>
        </LoadingProvider>
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}

export default App;
