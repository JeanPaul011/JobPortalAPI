JobPortalAPI

Project Overview

JobPortalAPI is a .NET 8.0 Web API that allows users to interact with a job portal system. It supports user authentication using JWT, job listings, company management, job applications, and reviews. The project is containerized with Docker and deployed on Azure with an SQL Server database.

Technologies Used

.NET 8.0 (ASP.NET Core Web API)

Entity Framework Core using SQLite for local development and SQL Server for production

JWT authentication

Docker for containerization

Azure Web App for deployment

Gmail SMTP for email notifications

IP-based rate limiting

Postman for API testing

Features

User authentication and role management for admin, recruiters, and job seekers

Job posting and applications allowing recruiters to post jobs and job seekers to apply

Company management where recruiters can create and manage companies

Reviews where users can leave feedback on companies

Email notifications for important events

Rate limiting to prevent API abuse

Project Structure

Controllers contain API endpoints for users, jobs, applications, companies, and reviews

Models define the database structure

DTOs are used for data transfer to avoid exposing sensitive fields

Repositories handle database queries

Services contain business logic such as email and user management

Migrations contain the database schema history

appsettings.json holds application configuration

.env file contains environment variables and is ignored in version control

Dockerfile sets up the container

Program.cs configures the API

Setup Instructions

Clone the repository and navigate into the project directory.

git clone https://github.com/yourusername/JobPortalAPI.git
cd JobPortalAPI

Create a .env file in the project root with the following variables:

SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_EMAIL=your-email@gmail.com
SMTP_PASSWORD=your-app-password
DB_CONNECTION=Server=tcp:jobportal-sqlserver.database.windows.net,1433;Initial Catalog=jobportal-db;Persist Security Info=False;User ID=adminuser;Password=YourStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
JWT_SECRET=YourSuperSecureJWTSecret
JWT_ISSUER=https://your-api.azurewebsites.net

Run the API locally using the following command:

dotnet run

The API will start on https://localhost:5276.

Build and run the Docker container:

docker build -t jobportal-api .
docker run -p 5276:5276 --env-file .env jobportal-api

Deploy to Azure using the following command:

az webapp up --name jobportal-api --resource-group JobPortalAPI-RG --runtime "DOTNETCORE|8.0" --os-type Windows

Ensure your Azure SQL database is set up before deployment.

API Endpoints

Authentication:

Register a new user at /api/auth/register

Login and get a JWT token at /api/auth/login

Users:

Get all users at /api/users (Admin only)

Get a user by ID at /api/users/{id}

Companies:

Create a new company at /api/companies (Recruiter only)

Get all companies at /api/companies

Jobs:

Create a job posting at /api/jobs

Get all jobs at /api/jobs

Job Applications:

Apply for a job at /api/jobapplications

Get all applications at /api/jobapplications (Admin only)

Reviews:

Submit a company review at /api/reviews

Get all reviews at /api/reviews

Troubleshooting

If you see JWT_SECRET is missing, ensure the .env file is loaded properly.

If you see Email Sending Failed: SMTP_EMAIL is missing, check that SMTP environment variables are correctly set.

If the database connection fails, ensure Azure SQL Server is correctly configured.

If you receive unauthorized access errors, make sure you include a valid Bearer token in API requests.

Submission Instructions

Ensure the API is successfully deployed to Azure.

Include all necessary files such as the .env file, database migrations, and the README.

If required, add API documentation and a Postman collection.

Perform a final test to confirm that all API endpoints function correctly.
