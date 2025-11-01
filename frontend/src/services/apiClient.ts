const apiClient = async (endpoint: string, options: RequestInit = {}) => {
  const apiUrl = process.env.REACT_APP_API_URL;

  if (!apiUrl) {
    throw new Error("API URL is not defined in environment variables");
  }

  const config: RequestInit = {
    ...options,
    credentials: "include", // Include cookies in requests
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  };

  const response = await fetch(`${apiUrl}${endpoint}`, config);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
};

export const streamApiClient = async (
  endpoint: string,
  options: RequestInit = {},
  onMessage: (data: {
    text: string;
    isComplete: boolean;
    error?: boolean;
  }) => void,
  onError?: (error: Error) => void,
  onComplete?: () => void
) => {
  const apiUrl = process.env.REACT_APP_API_URL;

  if (!apiUrl) {
    throw new Error("API URL is not defined in environment variables");
  }

  const config: RequestInit = {
    ...options,
    credentials: "include", // Include cookies in requests
    headers: {
      "Content-Type": "application/json",
      Accept: "text/event-stream",
      "Cache-Control": "no-cache",
      ...options.headers,
    },
  };

  try {
    const response = await fetch(`${apiUrl}${endpoint}`, config);

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    if (!response.body) {
      throw new Error("No response body");
    }

    const reader = response.body.getReader();
    const decoder = new TextDecoder();
    let buffer = "";

    try {
      while (true) {
        const { done, value } = await reader.read();

        if (done) {
          onComplete?.();
          break;
        }

        buffer += decoder.decode(value, { stream: true });
        const lines = buffer.split("\n");
        buffer = lines.pop() || ""; // Keep the last incomplete line in buffer

        for (const line of lines) {
          if (line.startsWith("data: ")) {
            try {
              const data = JSON.parse(line.slice(6));
              onMessage(data);
            } catch (parseError) {
              console.error("Error parsing SSE data:", parseError);
            }
          }
        }
      }
    } finally {
      reader.releaseLock();
    }
  } catch (error) {
    console.error("Streaming error:", error);
    onError?.(error as Error);
  }
};

export default apiClient;
