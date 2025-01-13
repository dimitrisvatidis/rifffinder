import React, { useEffect, useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";
import "./NavBar.css";

const Navbar = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [musician, setMusician] = useState(null);

  const handleLogout = () => {
    localStorage.removeItem("jwtToken");
    navigate("/login");
  };

  const fetchMusician = async () => {
    try {
      const token = localStorage.getItem("jwtToken");
      const response = await axios.get("https://localhost:7290/api/musicians/me", {
        headers: { Authorization: `Bearer ${token}` },
      });

      setMusician(response.data);
    } catch (error) {
      console.error("Error fetching musician details:", error);
    }
  };

  useEffect(() => {
    fetchMusician();
  }, [location.pathname]); 

  const isActive = (path) => (location.pathname === path ? "active" : "");

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm">
      <div className="container-fluid">
        <div className="d-flex align-items-center">
          <span className="navbar-brand fs-1 font-weight-bold rifffinder-title">
            RiffFinder
          </span>
        </div>

        <div className="d-flex justify-content-center flex-grow-1">
          <ul className="navbar-nav mb-2 mb-lg-0">
            <li className={`nav-item ${isActive("/")}`}>
              <Link to="/" className="nav-link fs-5">
                Home
              </Link>
            </li>
            <li className={`nav-item ${isActive("/profile")}`}>
              <Link to="/profile" className="nav-link fs-5">
                My Profile
              </Link>
            </li>
            {musician && musician.bandId && (
              <li className={`nav-item ${isActive("/my-band")}`}>
                <Link to="/my-band" className="nav-link fs-5">
                  My Band
                </Link>
              </li>
            )}
            <li className={`nav-item ${isActive("/requests")}`}>
              <Link to="/requests" className="nav-link fs-5">
                My Requests
              </Link>
            </li>
            {musician && !musician.bandId && (
              <li className={`nav-item ${isActive("/create-band")}`}>
                <Link to="/create-band" className="nav-link fs-5">
                  Create Band
                </Link>
              </li>
            )}
          </ul>
        </div>

        <div className="d-flex">
          <button onClick={handleLogout} className="btn btn-danger fs-5 px-4">
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;

