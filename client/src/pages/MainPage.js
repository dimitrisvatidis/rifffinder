import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import Footer from '../components/Footer';

const getStatusText = (status) => {
  switch (status) {
    case 0:
      return 'Open';
    case 1:
      return 'Closed';
    default:
      return 'Unknown';
  }
};

const MainPage = () => {
  const [postings, setPostings] = useState([]);
  const [belongsToBand, setBelongsToBand] = useState(false);
  const [loading, setLoading] = useState(true);
  const [musician, setMusician] = useState(null);
  const [userRequests, setUserRequests] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        const headers = { Authorization: `Bearer ${token}` };

        const postingsResponse = await axios.get(
          'https://localhost:7290/api/postings',
          { headers }
        );
        setPostings(postingsResponse.data);

        const musicianResponse = await axios.get(
          'https://localhost:7290/api/musicians/me',
          { headers }
        );
        const bandId = musicianResponse.data.bandId;
        setBelongsToBand(bandId !== null);
        setMusician(musicianResponse.data);

        const requestsResponse = await axios.get(
          'https://localhost:7290/api/requests',
          { headers }
        );
        setUserRequests(requestsResponse.data.map((req) => req.postingId));

        setLoading(false);
      } catch (error) {
        console.error('Error fetching data:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleInterestedClick = async (postingId) => {
    try {
      const token = localStorage.getItem('jwtToken');
      const headers = { Authorization: `Bearer ${token}` };

      await axios.post(
        `https://localhost:7290/api/requests`,
        { postingId },
        { headers }
      );

      alert('Request sent successfully!');
      setUserRequests((prev) => [...prev, postingId]);
    } catch (error) {
      console.error('Error sending request:', error);
      alert('Unable to send request.');
    }
  };

  const handleAddPosting = () => {
    navigate('/add-posting');
  };

  if (loading) return <p>Loading...</p>;

  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <div className="container mt-4 flex-grow-1">
        <div className="text-center mb-4">
          <h1 className="fw-bold">🎵 Available Postings</h1>
          <p className="text-muted">
            Find and explore the latest opportunities in the music world.
          </p>
        </div>

        {belongsToBand && (
          <div className="text-center mb-4">
            <button
              onClick={handleAddPosting}
              className="btn btn-primary btn-lg"
            >
              ➕ Add Posting
            </button>
          </div>
        )}

        <div className="row row-cols-1 row-cols-md-2 g-4">
          {postings
            .filter((posting) => posting.status === 0)
            .map((posting) => (
              <div key={posting.id} className="col">
                <div
                  className={`card shadow-sm h-100 border-0 rounded-4 ${
                    musician?.bandId === posting.bandId
                      ? 'border-primary border-2'
                      : ''
                  }`}
                >
                  <div className="card-body d-flex flex-column">
                    <h5 className="card-title text-primary fw-bold">
                      {posting.title}
                    </h5>
                    <p className="card-text text-muted mb-1">
                      <strong>Band:</strong> {posting.bandName || 'Unknown'}
                    </p>
                    <p className="card-text mb-1">
                      <strong>Instrument Wanted:</strong>{' '}
                      {posting.instrumentWanted}
                    </p>

                    <p className="card-text text-secondary mb-3">
                      {posting.text}
                    </p>

                    <p className="card-text">
                      <strong>Status:</strong>{' '}
                      <span
                        className={`badge ${posting.status === 0 ? 'bg-success' : 'bg-secondary'}`}
                      >
                        {getStatusText(posting.status)}
                      </span>
                    </p>

                    {musician?.bandId === posting.bandId && (
                      <span className="badge bg-info align-self-start mb-2">
                        Your Posting
                      </span>
                    )}

                    <div className="mt-auto d-flex flex-column gap-2">
                      <Link
                        to={`/bands/${posting.bandId}`}
                        className="btn btn-outline-primary btn-sm"
                      >
                        🎸 View Band
                      </Link>

                      {musician?.bandId !== posting.bandId && (
                        <>
                          <button
                            onClick={() => handleInterestedClick(posting.id)}
                            className="btn btn-success btn-sm"
                            disabled={userRequests.includes(posting.id)}
                          >
                            {userRequests.includes(posting.id)
                              ? 'Request Sent'
                              : "🤝 I'm Interested"}
                          </button>

                          {userRequests.includes(posting.id) && (
                            <Link
                              to="/requests"
                              className="btn btn-secondary btn-sm"
                            >
                              📄 View My Requests
                            </Link>
                          )}
                        </>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
        </div>

        {postings.filter((posting) => posting.status === 0).length === 0 && (
          <div className="text-center mt-5">
            <h5 className="text-muted">
              No open postings are available at the moment.
            </h5>
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
};

export default MainPage;
