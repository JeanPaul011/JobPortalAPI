# STEP 1: Build stage - Compiles the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore 

# Copy everything else and build the project
COPY . ./

# Ensure appsettings files are available in the build context BEFORE publishing
COPY appsettings.json /app/
COPY appsettings.Production.json /app/

# Ensure SQLite database is copied only if it exists
# This avoids errors if the file is missing
ARG DB_FILE=jobportal.db
RUN if [ -f "$DB_FILE" ]; then cp "$DB_FILE" /app/; fi

RUN dotnet publish -c Release -o /app/out

# STEP 2: Runtime stage - Runs the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

#  Copy the built files from the previous stage
COPY --from=build-env /app/out/ /app/

#  Ensure appsettings.json is included in the container
COPY --from=build-env /app/appsettings.json /app/
COPY --from=build-env /app/appsettings.Production.json /app/

#  Ensure SQLite database is copied only if it exists
ARG DB_FILE=jobportal.db
RUN if [ -f "$DB_FILE" ]; then cp "/app/$DB_FILE" /app/; fi

#  Set working directory
WORKDIR /app

# Expose the port (this must match your application settings)
EXPOSE 5276

#  Entry point (runs the application)
ENTRYPOINT ["dotnet", "JobPortalAPI.dll"]
