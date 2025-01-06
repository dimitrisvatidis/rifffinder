import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import Footer from '../components/Footer'; 
import 'bootstrap/dist/css/bootstrap.min.css';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('https://localhost:7290/api/login', {
        email,
        password,
      });

      const token = response.data.token;

      localStorage.setItem('jwtToken', token);

      navigate('/');
    } catch (err) {
      setError('Invalid email or password');
    }
  };

  useEffect(() => {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      navigate('/');
    }
  }, [navigate]);

  return (
    <div className="d-flex flex-column vh-100">
      <div className="d-flex justify-content-center align-items-center flex-grow-1 bg-light">
        <div className="card shadow-sm" style={{ width: '400px' }}>
          <div className="card-body">
            <h2 className="card-title text-center mb-4">Login</h2>
            {error && (
              <div className="alert alert-danger" role="alert">
                {error}
              </div>
            )}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  Email Address
                </label>
                <input
                  type="email"
                  id="email"
                  className="form-control"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
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
                  className="form-control"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
              <div className="d-grid">
                <button type="submit" className="btn btn-primary">
                  Login
                </button>
              </div>
            </form>
            <p className="text-center mt-3">
              Don’t have an account?{' '}
              <Link to="/register" className="text-primary text-decoration-underline">
                Register here
              </Link>
            </p>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default Login;
