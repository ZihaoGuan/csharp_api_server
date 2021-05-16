# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:latest AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY * ./
#RUN dotnet publish -c Release -o out
RUN dotnet publish --configuration Release -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AgentApi.dll"]