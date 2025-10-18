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
import { PublicRoutes } from "./constants/routes";

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
              {/* Public Routes */}
              <Route path={PublicRoutes.LOGIN} element={<Login />} />
              <Route path={PublicRoutes.ERROR} element={<ErrorPage />} />
              <Route
                path={PublicRoutes.UNKNOWN}
                element={<p>There's nothing here</p>}
              />

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
