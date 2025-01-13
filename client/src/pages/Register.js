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

  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState('');
  const [serverError, setServerError] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const validate = () => {
    const validationErrors = {};

    
    if (!formData.name.trim()) {
      validationErrors.name = 'Name is required.';
    }

    
    if (!formData.surname.trim()) {
      validationErrors.surname = 'Surname is required.';
    }

    
    if (!formData.email) {
      validationErrors.email = 'Email is required.';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      validationErrors.email = 'Please enter a valid email address.';
    }

    
    if (!formData.password) {
      validationErrors.password = 'Password is required.';
    }

    
    if (!formData.instrument.trim()) {
      validationErrors.instrument = 'Instrument is required.';
    }

    setErrors(validationErrors);
    return Object.keys(validationErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    try {
      const response = await axios.post('https://localhost:7290/api/musicians', formData);
      console.log('Registration successful:', response.data);

      setSuccessMessage('Registration successful! Redirecting to login...');
      setServerError('');
      setErrors({});

      setTimeout(() => navigate('/login'), 3000);
    } catch (err) {
      if (err.response && err.response.status === 500) { 
        setServerError('A user with this email already exists.');
      } else {
        setServerError('Failed to register. Please try again.');
      }
      setSuccessMessage('');
    }
  };

  return (
    <div className="d-flex flex-column vh-100">
      <div className="d-flex justify-content-center align-items-center flex-grow-1 bg-light">
        <div className="card shadow-sm" style={{ width: '400px' }}>
          <div className="card-body">
            <h2 className="card-title text-center mb-4">Register</h2>
            {serverError && (
              <div className="alert alert-danger" role="alert">
                {serverError}
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
                  className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                  value={formData.name}
                  onChange={handleChange}
                />
                {errors.name && <div className="invalid-feedback">{errors.name}</div>}
              </div>

              
              <div className="mb-3">
                <label htmlFor="surname" className="form-label">
                  Surname
                </label>
                <input
                  type="text"
                  id="surname"
                  name="surname"
                  className={`form-control ${errors.surname ? 'is-invalid' : ''}`}
                  value={formData.surname}
                  onChange={handleChange}
                />
                {errors.surname && <div className="invalid-feedback">{errors.surname}</div>}
              </div>

              
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  Email
                </label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                  value={formData.email}
                  onChange={handleChange}
                />
                {errors.email && <div className="invalid-feedback">{errors.email}</div>}
              </div>

              
              <div className="mb-3">
                <label htmlFor="password" className="form-label">
                  Password
                </label>
                <input
                  type="password"
                  id="password"
                  name="password"
                  className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                  value={formData.password}
                  onChange={handleChange}
                />
                {errors.password && <div className="invalid-feedback">{errors.password}</div>}
              </div>

              
              <div className="mb-3">
                <label htmlFor="instrument" className="form-label">
                  Instrument
                </label>
                <input
                  type="text"
                  id="instrument"
                  name="instrument"
                  className={`form-control ${errors.instrument ? 'is-invalid' : ''}`}
                  value={formData.instrument}
                  onChange={handleChange}
                />
                {errors.instrument && <div className="invalid-feedback">{errors.instrument}</div>}
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
