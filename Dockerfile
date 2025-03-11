# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore 

# Copy everything else and build the project
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app and config files
COPY --from=build-env /app/out/ ./
COPY --from=build-env /app/appsettings.json ./
COPY --from=build-env /app/appsettings.Production.json ./

# Configuration
ENV ASPNETCORE_URLS=http://+:5276
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5276

# Run the application
ENTRYPOINT ["dotnet", "JobPortalAPI.dll"]