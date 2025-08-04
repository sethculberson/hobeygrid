###Hobey Grid
#Full-Stack College Hockey Trivia Application

This is a full-stack web application for the Hobey Grid, a daily trivia game where users must find NCAA hockey players that fit intersecting categories. The project is a demonstration of a modern, containerized, and cloud-native application, showcasing skills in full-stack development, data pipelining, and automated deployment.

#Project Highlights

Full-Stack Architecture: The application consists of a ReactJS frontend for an interactive user experience and an ASP.NET Core Web API backend for all business logic.

Data Pipeline: A Python script is used to scrape and import 15 seasons of NCAA player statistics into a PostgreSQL database.

Dynamic Grid Generation: The backend API dynamically generates daily grids by interpreting user-defined category templates and performing complex database queries to find all valid player intersections.

Containerization: The entire application stack (frontend, backend, and database) is containerized with Docker and orchestrated using Docker Compose for a consistent development environment.

Cloud Deployment: The application is deployed to Microsoft Azure, utilizing Azure App Service and Azure Database for PostgreSQL. A GitHub Actions workflow automates the CI/CD pipeline.

#Repository Structure

This repository is organized into two primary folders, each containing a separate application:

hobey-grid-frontend/: The ReactJS frontend application.

HobeyGridApi/: The ASP.NET Core Web API backend.

#Getting Started

Clone the repository:

git clone https://github.com/sethculberson/hobeygrid.git
cd hobeygrid

Start the backend:

cd HobeyGridApi
dotnet build
dotnet start

In a second terminal window, start the frontend:

cd hobey-grid-frontend
npm start

Access the applications:

Frontend: http://localhost:3000

Backend (Swagger UI): http://localhost:7001/swagger

Enjoy the game!