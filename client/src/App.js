// src/App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes, Outlet } from 'react-router-dom';
import {Register, Login, MainPage, Band, Musician, Requests, CreateBand, AddPosting } from './pages';
import { ProtectedRoute, NavBar, } from './components';

const App = () => {
  return (
    <Router>
      <div>
        <Routes>
          <Route path="/register" element={<Register />} />
          <Route path="/login" element={<Login />} />
        <Route
          element={
            <ProtectedRoute>
              <NavBar />
              <Outlet />
            </ProtectedRoute>
          }
        >
          <Route path="/" element={<MainPage />} />
          <Route path="/profile" element={<Musician />} />
          <Route path="/musicians/:id" element={<Musician />} />
          <Route path="/bands/:id" element={<Band />} />
          <Route path="/my-band" element={<Band/>} />
          <Route path="/requests" element={<Requests />} />
          <Route path="/create-band" element={<CreateBand />} />
          <Route path="/add-posting" element={<AddPosting />} />
        </Route>
        </Routes>
      </div>
    </Router>
  );
};

export default App;

