const apiClient = async (endpoint: string, options: RequestInit = {}) => {
  const apiUrl = process.env.REACT_APP_API_URL;

  if (!apiUrl) {
    throw new Error("API URL is not defined in environment variables");
  }

  const jwtToken = sessionStorage.getItem("jwtToken");

  const defaultHeaders = {
    "Content-Type": "application/json",
    ...(jwtToken && { Authorization: `Bearer ${jwtToken}` }),
  };

  const config: RequestInit = {
    ...options,
    headers: {
      ...defaultHeaders,
      ...options.headers,
    },
  };

  const response = await fetch(`${apiUrl}${endpoint}`, config);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
};

export default apiClient;
