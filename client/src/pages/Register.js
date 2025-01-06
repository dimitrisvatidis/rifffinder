import React, { useState } from 'react';
import axios from 'axios';
import { Link, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import Footer from '../components/Footer'; 

const Register = () => {
  const [formData, setFormData] = useState({
    name: '',
    surname: '',
    email: '',
    password: '',
    instrument: '',
  });

  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('https://localhost:7290/api/musicians', formData);
      console.log('Registration successful:', response.data);

      setSuccessMessage('Registration successful! Redirecting to login...');
      setError('');

      setTimeout(() => navigate('/login'), 3000);
    } catch (err) {
      console.error('Error registering musician:', err);
      setError('Failed to register. Please try again.');
      setSuccessMessage('');
    }
  };

  return (
    <div className="d-flex flex-column vh-100">
      <div className="d-flex justify-content-center align-items-center flex-grow-1 bg-light">
        <div className="card shadow-sm" style={{ width: '400px' }}>
          <div className="card-body">
            <h2 className="card-title text-center mb-4">Register</h2>
            {error && (
              <div className="alert alert-danger" role="alert">
                {error}
              </div>
            )}
            {successMessage && (
              <div className="alert alert-success" role="alert">
                {successMessage}
              </div>
            )}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label htmlFor="name" className="form-label">
                  Name
                </label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  className="form-control"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="surname" className="form-label">
                  Surname
                </label>
                <input
                  type="text"
                  id="surname"
                  name="surname"
                  className="form-control"
                  value={formData.surname}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  Email
                </label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  className="form-control"
                  value={formData.email}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="password" className="form-label">
                  Password
                </label>
                <input
                  type="password"
                  id="password"
                  name="password"
                  className="form-control"
                  value={formData.password}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="instrument" className="form-label">
                  Instrument
                </label>
                <input
                  type="text"
                  id="instrument"
                  name="instrument"
                  className="form-control"
                  value={formData.instrument}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="d-grid">
                <button type="submit" className="btn btn-primary">
                  Register
                </button>
              </div>
            </form>
            <p className="text-center mt-3">
              Already have an account?{' '}
              <Link to="/login" className="text-primary text-decoration-underline">
                Login here
              </Link>
            </p>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default Register;
