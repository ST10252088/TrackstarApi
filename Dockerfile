# Use official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./Trackstar.Api ./Trackstar.Api
WORKDIR /app/Trackstar.Api
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Trackstar.Api/out .
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENTRYPOINT ["dotnet", "Trackstar.Api.dll"]
