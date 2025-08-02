Hobey Grid Frontend
College Hockey Trivia Web Application

This is the frontend for the Hobey Grid, a daily web application for testing college hockey knowledge. Inspired by popular trivia games like HoopGrids and Immaculate Grid, the application presents a dynamic 3x3 grid where users must find players who fit intersecting categories.

The frontend is built with modern technologies to provide a fast, responsive, and engaging user experience.

<br>

Key Features

Interactive 3x3 Grid: A clean and intuitive interface for entering player guesses.

Dynamic Categories: Fetches daily row and column categories from a backend API.

Player Search & Autocomplete: As a user types, the application suggests matching players from the database.

Backend Validation: Submits the completed grid to a backend API for validation and displays results for each cell.

Responsive Design: Built using Tailwind CSS, ensuring a seamless experience across desktop, tablet, and mobile devices.

Modern Component Architecture: The application is built with React hooks and a modular component structure for maintainability and scalability.

<br>

Technologies Used

ReactJS: A declarative, component-based JavaScript library for building user interfaces.

Tailwind CSS: A utility-first CSS framework for building custom designs rapidly.

lucide-react: A collection of beautiful and customizable open-source icons.

JavaScript (ES6+): For core logic and asynchronous operations.

Vite or Create React App: The project's build toolchain.

<br>

Project Setup and Installation

Follow these steps to set up the project on your local machine. This assumes you have Node.js and npm installed.

1. Clone the repository

git clone https://github.com/your-username/hobey-grid.git
cd hobey-grid/hobey-grid-frontend

2. Install dependencies

npm install

3. Configure API Endpoint

The frontend needs to know where your backend API is running.

Open src/hooks/useHobeyGridGame.js.

Update the API_BASE_URL constant to match your backend's URL.

// src/hooks/useHobeyGridGame.js
const API_BASE_URL = 'https://localhost:7001'; // Or your deployed Azure App Service URL

4. Run the development server

npm start

The application will be available in your browser at http://localhost:3000.

<br>

Directory Structure

The project is organized into a clean, component-based architecture.

hobey-grid-frontend/
├── public/
├── src/
│   ├── components/
│   │   ├── GameControls.js
│   │   ├── GridCellInput.js
│   │   └── GridDisplay.js
│   ├── hooks/
│   │   └── useHobeyGridGame.js
│   ├── App.js
│   └── index.css
├── package.json
├── tailwind.config.js
└── postcss.config.js

<br>

Contributions

Contributions, bug reports, and feature requests are welcome!

<br>

© {current_year} Hobey Grid. All rights reserved.
Inspired by HoopGrids and Immaculate Grid.