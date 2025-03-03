Overview

JobPortalAPI is a .NET 8.0 Web API designed for job management, allowing users to register, post jobs, apply for jobs, and leave reviews for companies. The API provides user authentication, job management, and company management. The project is structured to be run locally with SQLite and deployed on Azure through Visual Studio Code.

How to Run

Open the project in Visual Studio Code.

Ensure that you have the necessary dependencies installed by running dotnet restore.

Start the API by running dotnet run in the terminal.

The API will start on https://localhost:5276.

Open a browser and navigate to https://localhost:5276/swagger to test endpoints.

How to Use Swagger

Swagger allows users to interact with the API without needing a separate client application.

Open https://localhost:5276/swagger in a browser.

Use the POST /api/auth/register endpoint to create a user.

Use the POST /api/auth/login endpoint to generate a JWT token.

Copy the token and click Authorize in Swagger to enable protected endpoints.

Use the various GET, POST, PUT, DELETE endpoints to interact with jobs, companies, applications, and reviews.

API Functions

User Authentication

Register new users.

Login to receive a JWT token.

Job Management

Create and view job postings.

Apply for jobs.

Company Management

Add and view companies.

Reviews

Users can submit reviews for companies.

Folder Structure

Controllers

Handles API requests for users, jobs, applications, companies, and reviews.

Each controller maps to a specific resource and processes incoming HTTP requests.

Models

Defines the structure of database entities like Users, Jobs, Applications, Companies, and Reviews.

DTOs

Data Transfer Objects (DTOs) ensure that only necessary data is exposed in API responses.

Repositories

Handles database operations using Entity Framework.

Contains query logic for fetching and storing data.

Services

Contains business logic separate from controllers.

Handles email sending, job applications, and general user interactions.

Migrations

Tracks database schema changes.

Contains scripts for updating the database structure.

appsettings.json

Stores configuration settings for database connections and JWT authentication.

ConnectionStrings contains the SQLite connection string for local development.

Jwt section includes the secret key and issuer information for authentication.

EmailSettings configures the SMTP server for email notifications.

Program.cs

Configures dependency injection for repositories and services.

Sets up authentication and authorization.

Enables API routing and Swagger UI.

How to Deploy on Azure

Open Visual Studio Code and ensure the Azure extension is installed.

Click on the Azure section and sign in to your Azure account.

Create a new Azure Web App by providing a name and selecting .NET 8.0.

Deploy the project using the Azure section in Visual Studio Code.

Check the Microsoft Azure Resource Groups to confirm the API is running.

Click on the deployed JobPortalAPI link to test it.

Troubleshooting

If the API does not start, check that all dependencies are installed by running dotnet restore.

If Swagger does not show endpoints, ensure the correct port is being used.

If email notifications fail, verify the SMTP settings in appsettings.json and environment variables.

If the database is not loading, confirm that migrations are applied using dotnet ef database update.
