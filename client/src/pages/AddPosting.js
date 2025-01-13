import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import Footer from '../components/Footer';

const AddPosting = () => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [instrumentWanted, setInstrumentWanted] = useState('');
  const [bandId, setBandId] = useState(null);
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchMusicianDetails = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        const response = await axios.get(
          'https://localhost:7290/api/musicians/me',
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );
        setBandId(response.data.bandId);
      } catch (error) {
        console.error('Error fetching musician details:', error);
        setServerError(
          'Unable to fetch your band details. Please try again later.'
        );
      }
    };
    fetchMusicianDetails();
  }, []);

  const validate = () => {
    const validationErrors = {};

    if (!title.trim()) {
      validationErrors.title = 'Title is required.';
    }
    if (!description.trim()) {
      validationErrors.description = 'Description is required.';
    }
    if (!instrumentWanted.trim()) {
      validationErrors.instrumentWanted = 'Instrument Wanted is required.';
    }

    setErrors(validationErrors);
    return Object.keys(validationErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    if (!bandId) {
      alert('You are not associated with a band. Cannot create a posting.');
      return;
    }

    try {
      const token = localStorage.getItem('jwtToken');
      await axios.post(
        'https://localhost:7290/api/postings',
        { title, text: description, instrumentWanted, bandId },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      alert('Posting created successfully!');
      navigate('/');
    } catch (error) {
      console.error('Error creating posting:', error);
      setServerError('Failed to create posting. Please try again.');
    }
  };

  return (
    <div className="d-flex flex-column min-vh-100">
      <div className="container mt-5 flex-grow-1">
        <h1 className="text-center mb-4">Add New Posting</h1>
        {serverError && (
          <div className="alert alert-danger" role="alert">
            {serverError}
          </div>
        )}
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label" style={{ fontSize: '1.25rem' }}>
              Title
            </label>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className={`form-control ${errors.title ? 'is-invalid' : ''}`}
              style={{ fontSize: '1.2rem' }}
            />
            {errors.title && (
              <div className="invalid-feedback">{errors.title}</div>
            )}
          </div>

          <div className="mb-3">
            <label className="form-label" style={{ fontSize: '1.25rem' }}>
              Description
            </label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className={`form-control ${errors.description ? 'is-invalid' : ''}`}
              style={{ fontSize: '1.2rem' }}
            ></textarea>
            {errors.description && (
              <div className="invalid-feedback">{errors.description}</div>
            )}
          </div>

          <div className="mb-3">
            <label className="form-label" style={{ fontSize: '1.25rem' }}>
              Instrument Wanted
            </label>
            <input
              type="text"
              value={instrumentWanted}
              onChange={(e) => setInstrumentWanted(e.target.value)}
              className={`form-control ${errors.instrumentWanted ? 'is-invalid' : ''}`}
              style={{ fontSize: '1.2rem' }}
            />
            {errors.instrumentWanted && (
              <div className="invalid-feedback">{errors.instrumentWanted}</div>
            )}
          </div>

          <button
            type="submit"
            className="btn btn-primary w-100"
            style={{ fontSize: '1.2rem' }}
          >
            Create Posting
          </button>
        </form>
      </div>
      <Footer />
    </div>
  );
};

export default AddPosting;
