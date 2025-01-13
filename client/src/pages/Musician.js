import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import Footer from '../components/Footer';

const Musician = () => {
  const { id } = useParams();
  const [musician, setMusician] = useState(null);
  const [bandName, setBandName] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchMusician = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        const headers = { Authorization: `Bearer ${token}` };

        let musicianResponse;

        if (id) {
          musicianResponse = await axios.get(
            `https://localhost:7290/api/musicians/${id}`,
            { headers }
          );
        } else {
          musicianResponse = await axios.get(
            'https://localhost:7290/api/musicians/me',
            { headers }
          );
        }

        console.log(musicianResponse);
        const musicianData = musicianResponse.data;
        setMusician(musicianData);

        if (musicianData.bandId) {
          const bandResponse = await axios.get(
            `https://localhost:7290/api/bands/${musicianData.bandId}`,
            { headers }
          );
          setBandName(bandResponse.data.name);
        }
      } catch (error) {
        console.error('Error fetching musician or band details:', error);
        setError('Failed to fetch musician details.');
      }
    };

    fetchMusician();
  }, [id]);

  if (error) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="text-danger">{error}</div>
      </div>
    );
  }

  if (!musician) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  console.log(musician);
  return (
    <div className="d-flex flex-column min-vh-100">
      <div className="container mt-5 flex-grow-1">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card shadow-sm">
              <div className="card-header bg-dark text-white text-center">
                <h2>{id ? 'Musician Profile' : 'My Profile'}</h2>
              </div>
              <div className="card-body">
                <p className="mb-3">
                  <strong>Name:</strong> {musician.name}
                </p>
                <p className="mb-3">
                  <strong>Surname:</strong> {musician.surname}
                </p>
                <p className="mb-3">
                  <strong>Email:</strong> {musician.email}
                </p>
                <p className="mb-3">
                  <strong>Instrument:</strong> {musician.instrument}
                </p>
                {musician.bandId ? (
                  <p className="mb-3">
                    <strong>Band:</strong>{' '}
                    {bandName ? (
                      <span>{bandName}</span>
                    ) : (
                      <span>Loading band details...</span>
                    )}
                  </p>
                ) : (
                  <p className="text-muted">
                    This musician is not part of a band.
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default Musician;
