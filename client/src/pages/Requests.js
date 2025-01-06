import React, { useEffect, useState } from "react";
import axios from "axios";
import Footer from "../components/Footer";

const getStatusText = (status) => {
  switch (status) {
    case 0:
      return "Pending";
    case 1:
      return "Accepted";
    case 2:
      return "Denied";
    default:
      return "Unknown";
  }
};

const getStatusBadgeClass = (status) => {
  switch (status) {
    case 0:
      return "badge bg-warning";
    case 1:
      return "badge bg-success";
    case 2:
      return "badge bg-danger";
    default:
      return "badge bg-secondary";
  }
};

const Requests = () => {
  const [requests, setRequests] = useState([]);
  const [musician, setMusician] = useState(null);
  const [musiciansData, setMusiciansData] = useState({});
  const [postingsData, setPostingsData] = useState({});
  const [bandsData, setBandsData] = useState({});

  useEffect(() => {
    const fetchRequests = async () => {
      try {
        const token = localStorage.getItem("jwtToken");
        const headers = { Authorization: `Bearer ${token}` };

        const musicianResponse = await axios.get(
          "https://localhost:7290/api/musicians/me",
          { headers }
        );
        setMusician(musicianResponse.data);

        const requestsResponse = await axios.get(
          "https://localhost:7290/api/requests",
          { headers }
        );
        setRequests(requestsResponse.data);

        const musicianIds = [...new Set(requestsResponse.data.map((req) => req.musicianId))];
        const postingIds = [...new Set(requestsResponse.data.map((req) => req.postingId))];
        const bandIds = [...new Set(requestsResponse.data.map((req) => req.bandId))];

        const musicianDetails = await Promise.all(
          musicianIds.map(async (id) => {
            const res = await axios.get(`https://localhost:7290/api/musicians/${id}`, { headers });
            return { id, ...res.data };
          })
        );
        setMusiciansData(
          musicianDetails.reduce((acc, m) => ({ ...acc, [m.id]: m }), {})
        );

        const postingDetails = await Promise.all(
          postingIds.map(async (id) => {
            const res = await axios.get(`https://localhost:7290/api/postings/${id}`, { headers });
            return { id, ...res.data };
          })
        );
        setPostingsData(
          postingDetails.reduce((acc, p) => ({ ...acc, [p.id]: p }), {})
        );

        const bandDetails = await Promise.all(
          bandIds.map(async (id) => {
            const res = await axios.get(`https://localhost:7290/api/bands/${id}`, { headers });
            return { id, ...res.data };
          })
        );
        setBandsData(
          bandDetails.reduce((acc, b) => ({ ...acc, [b.id]: b }), {})
        );
      } catch (error) {
        console.error("Error fetching requests or related data:", error);
      }
    };

    fetchRequests();
  }, []);

  const handleAction = async (requestId, action) => {
    try {
      const token = localStorage.getItem("jwtToken");
      const urlAction = action === 'Approved' ? 'accept' : 'deny';

      await axios.patch(
        `https://localhost:7290/api/requests/${requestId}/${urlAction}`,
        {},
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setRequests((prev) =>
        prev.map((req) =>
          req.id === requestId
            ? { ...req, status: action === "Approved" ? 1 : 2 }
            : req
        )
      );

      alert(`Request ${action.toLowerCase()} successfully!`);
    } catch (error) {
      console.error(`Error updating request status:`, error);
      alert("Unable to update request.");
    }
  };

  const pendingIncomingRequests = requests.filter(
    (req) => req.status === 0 && req.bandId === musician?.bandId
  );

  const pendingOutgoingRequests = requests.filter(
    (req) => req.status === 0 && req.musicianId === musician?.id
  );

  const completedRequests = requests.filter((req) =>
    musician?.bandId
      ? req.bandId === musician.bandId && (req.status === 1 || req.status === 2)
      : req.musicianId === musician?.id && (req.status === 1 || req.status === 2)
  );

  return (
    <div className="d-flex flex-column min-vh-100">
    <div className="container mt-5 flex-grow-1">
      <h1 className="text-center mb-5 display-4 fw-bold">Requests</h1>
          <h2 className="mb-4 fw-bold fs-3">📥 Pending Incoming Requests</h2>
          {pendingIncomingRequests.map((req) => (
            <div key={req.id} className="card shadow-lg mb-4 border-0">
              <div className="card-header bg-dark text-white fs-5 fw-semibold">
                {musiciansData[req.musicianId]?.name || "Unknown"} wants to join your band
              </div>
              <div className="card-body fs-5">
                <p><strong>🎸 Instrument:</strong> {musiciansData[req.musicianId]?.instrument || "Unknown"}</p>
                <p><strong>📄 Posting:</strong> {postingsData[req.postingId]?.title || "Unknown"}</p>
                <div className="d-flex gap-2 mb-3">
                  <a
                    href={`/musicians/${req.musicianId}`}
                    className="btn btn-info fw-bold"
                  >
                    👤 View Musician
                  </a>
                </div>
                <div className="d-flex gap-2">
                  <button onClick={() => handleAction(req.id, "Approved")} className="btn btn-lg btn-success fw-bold">
                    ✅ Approve
                  </button>
                  <button onClick={() => handleAction(req.id, "Rejected")} className="btn btn-lg btn-danger fw-bold">
                    ❌ Deny
                  </button>
                </div>
              </div>
            </div>
          ))}

        <h2 className="mb-4 fw-bold fs-3">🚀 Pending Outgoing Requests</h2>
        {pendingOutgoingRequests.map((req) => (
          <div key={req.id} className="card shadow-lg mb-4 border-0">
            <div className="card-header bg-secondary text-white fs-5 fw-semibold">
              You have requested to join {bandsData[req.bandId]?.name || "Unknown"}
            </div>
            <div className="card-body fs-5">
              <p><strong>📄 Posting Title:</strong> {postingsData[req.postingId]?.title || "Unknown"}</p>
              <p>
                <strong>Status:</strong>{" "}
                <span className={`badge ${getStatusBadgeClass(req.status)} fs-6`}>
                  {getStatusText(req.status)}
                </span>
              </p>
              <div className="d-flex gap-2">
                <a
                  href={`/bands/${req.bandId}`}
                  className="btn btn-info fw-bold"
                >
                  🎵 View Band
                </a>
              </div>
            </div>
          </div>
        ))}

        <h2 className="mb-4 fw-bold fs-3">✅ Completed Requests</h2>
        {completedRequests.map((req) => (
          <div key={req.id} className="card shadow-lg mb-4 border-0">
            <div className={`card-header ${getStatusBadgeClass(req.status)} text-white fs-5 fw-semibold`}>
              {musiciansData[req.musicianId]?.name || "Unknown"},  
              ({musiciansData[req.musicianId]?.instrument || "Unknown"}), 
              applied for {postingsData[req.postingId]?.title || "Unknown"}, 
              for the band {bandsData[req.bandId]?.name || "Unknown"}
            </div>
            <div className="card-body fs-5">
              <p><strong>Status:</strong> {getStatusText(req.status)}</p>
            </div>
          </div>
        ))}
      </div>
    <Footer />
  </div>
  
  );
};

export default Requests;
