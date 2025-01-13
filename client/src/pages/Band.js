import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import Footer from '../components/Footer';

const Band = () => {
  const { id } = useParams();
  const [band, setBand] = useState(null);
  const [musicians, setMusicians] = useState([]);
  const [error, setError] = useState(null);
  const [isMyBand, setIsMyBand] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBandDetails = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        const headers = { Authorization: `Bearer ${token}` };

        let bandIdToFetch = id;

        if (!id) {
          const musicianResponse = await axios.get(
            'https://localhost:7290/api/musicians/me',
            { headers }
          );
          bandIdToFetch = musicianResponse.data.bandId;
          setIsMyBand(true);
        }

        const bandResponse = await axios.get(
          `https://localhost:7290/api/bands/${bandIdToFetch}`,
          { headers }
        );

        setBand(bandResponse.data);

        const musiciansResponse = await axios.get(
          `https://localhost:7290/api/musicians?bandId=${bandIdToFetch}`,
          { headers }
        );
        setMusicians(musiciansResponse.data);
      } catch (err) {
        console.error('Error fetching band details:', err);
        setError('Error fetching band details.');
      }
    };

    fetchBandDetails();
  }, [id]);

  const handleLeaveBand = async () => {
    try {
      const token = localStorage.getItem('jwtToken');
      await axios.put(
        `https://localhost:7290/api/bands/leave-band`,
        {},
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      alert('You have successfully left the band.');
      navigate('/profile');
    } catch (error) {
      console.error('Error leaving the band:', error);
      alert('Unable to leave the band. Please try again.');
    }
  };

  if (error) return <div>{error}</div>;
  if (!band)
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );

  return (
    <div className="d-flex flex-column min-vh-100">
      <div className="container mt-5 flex-grow-1">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card shadow-sm">
              <div className="card-header bg-dark text-white text-center">
                <h2>{isMyBand ? 'My Band' : 'Band Details'}</h2>
              </div>
              <div className="card-body">
                <p>
                  <strong>Name:</strong> {band.name}
                </p>
                <p>
                  <strong>Genre:</strong> {band.genre}
                </p>
                <p>
                  <strong>Bio:</strong> {band.bio}
                </p>
                <h4 className="mt-4">Musicians</h4>
                <ul className="list-group">
                  {musicians.length > 0 ? (
                    musicians.map((musician) => (
                      <li key={musician.id} className="list-group-item">
                        {musician.name} {musician.surname} -{' '}
                        {musician.instrument}
                      </li>
                    ))
                  ) : (
                    <li className="list-group-item">
                      No musicians in this band.
                    </li>
                  )}
                </ul>
              </div>
            </div>
            {isMyBand && (
              <div className="text-center mt-4">
                <button className="btn btn-danger" onClick={handleLeaveBand}>
                  Leave Band
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default Band;
