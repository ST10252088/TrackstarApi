# Use official .NET 8 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything (since .csproj is in repo root)
COPY . .
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Render automatically sets the PORT variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "Trackstar.Api.dll"]
