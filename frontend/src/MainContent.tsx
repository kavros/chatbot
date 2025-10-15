import { useEffect, useState } from "react";

import logo from "./logo.svg";
import "./App.css";
import apiClient from "./services/apiClient";
import { useLoading } from "./loading/LoadingContext";

interface Owner {
  id: number;
  name: string;
  city: string;
  telephone?: string; // Added telephone property
}
function MainContent() {
  const [owners, setOwners] = useState<Owner[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const { setLoading } = useLoading(); // Use global loading context

  useEffect(() => {
    setLoading(true); // Show global loading spinner

    apiClient("owners")
      .then((data: Owner[]) => {
        setOwners(data);
        setLoading(false); // Hide global loading spinner
      })
      .catch((error) => {
        console.error("Error fetching the owners:", error);
        setError("Error fetching the owners");
        setLoading(false); // Hide global loading spinner
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />

        {error || !owners ? (
          <p>{error}</p>
        ) : (
          <table className="owners-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>City</th>
                <th>Telephone</th>
              </tr>
            </thead>
            <tbody>
              {owners.map((owner) => (
                <tr key={owner.id}>
                  <td>{owner.id}</td>
                  <td>{owner.name}</td>
                  <td>{owner.city}</td>
                  <td>{owner.telephone ?? "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </header>
    </div>
  );
}

export default MainContent;
