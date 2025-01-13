import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import Footer from "../components/Footer";

const CreateBand = () => {
  const [bandData, setBandData] = useState({ name: "", bio: "", genre: "" });
  const [errors, setErrors] = useState({});
  const [musician, setMusician] = useState(null);
  const [serverError, setServerError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
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

    fetchMusician();
  }, []);

  const validate = () => {
    const validationErrors = {};
    if (!bandData.name.trim()) {
      validationErrors.name = "Band name is required.";
    }
    if (!bandData.bio.trim()) {
      validationErrors.bio = "Band bio is required.";
    }
    if (!bandData.genre.trim()) {
      validationErrors.genre = "Band genre is required.";
    }
    setErrors(validationErrors);
    return Object.keys(validationErrors).length === 0;
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setBandData((prevData) => ({ ...prevData, [name]: value }));
    setErrors((prevErrors) => ({ ...prevErrors, [name]: "" })); 
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    try {
      const token = localStorage.getItem("jwtToken");
      await axios.post("https://localhost:7290/api/bands", bandData, {
        headers: { Authorization: `Bearer ${token}` },
      });

      navigate("/my-band");
    } catch (error) {
      console.error("Error creating band:", error);
      setServerError("Failed to create the band. Please try again.");
    }
  };

  if (musician === null) {
    return <div>Loading...</div>;
  }

  if (musician.bandId) {
    return <div>You are already part of a band and cannot create a new one.</div>;
  }

  return (
    <div className="d-flex flex-column min-vh-100">
      <div className="container mt-5 flex-grow-1">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card shadow-sm">
              <div className="card-header bg-dark text-white text-center">
                <h2>Create Band</h2>
              </div>
              <div className="card-body">
                {serverError && (
                  <div className="alert alert-danger" role="alert">
                    {serverError}
                  </div>
                )}
                <form onSubmit={handleSubmit}>
                  
                  <div className="mb-3">
                    <label className="form-label">Name:</label>
                    <input
                      type="text"
                      name="name"
                      value={bandData.name}
                      onChange={handleInputChange}
                      className={`form-control ${errors.name ? "is-invalid" : ""}`}
                    />
                    {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                  </div>

                  
                  <div className="mb-3">
                    <label className="form-label">Bio:</label>
                    <textarea
                      name="bio"
                      value={bandData.bio}
                      onChange={handleInputChange}
                      className={`form-control ${errors.bio ? "is-invalid" : ""}`}
                    ></textarea>
                    {errors.bio && <div className="invalid-feedback">{errors.bio}</div>}
                  </div>

                 
                  <div className="mb-3">
                    <label className="form-label">Genre:</label>
                    <input
                      type="text"
                      name="genre"
                      value={bandData.genre}
                      onChange={handleInputChange}
                      className={`form-control ${errors.genre ? "is-invalid" : ""}`}
                    />
                    {errors.genre && <div className="invalid-feedback">{errors.genre}</div>}
                  </div>

                  <button type="submit" className="btn btn-primary w-100">
                    Create Band
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default CreateBand;
