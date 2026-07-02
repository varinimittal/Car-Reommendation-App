Car Recommendation System

This is a full-stack web application that recommends cars based on user input such as budget and preferences. The project is built using Angular for the frontend and .NET Web API for the backend.

Project Structure

Car-Recommendation-System
- frontend (Angular application)
- backend (.NET Web API)
- README.md

Prerequisites

Make sure the following are installed on your system:
- Node.js (v16 or above)
- Angular CLI (npm install -g @angular/cli)
- .NET SDK (v6 or above)

How to Run the Project

1. Run Backend

Open terminal and navigate to the backend folder:

cd backend

Restore dependencies:

dotnet restore

Run the backend server:

dotnet run

The backend will start on:
https://localhost:7240

2. Run Frontend

Open a new terminal and navigate to the frontend folder:

cd frontend

Install dependencies:

npm install

Run the Angular application:

ng serve

Open the application in browser:
http://localhost:4200

Configuration

Ensure that the frontend is connected to the backend API. Update the API base URL in the Angular service file if required:

baseUrl = 'https://localhost:5001/api';

Important Notes

- Start the backend before running the frontend
- Ensure ports match between frontend and backend
- Enable CORS in backend if API requests fail

Features

- Recommend cars based on user input
- Filter cars by price and preferences
- Frontend and backend integration using REST API

Future Improvements

- Add database integration
- Improve recommendation logic (range-based filtering instead of exact match)
- Add authentication and better UI

Author

Varini Mittal
