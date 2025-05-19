
# Use official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy and restore
COPY . ./
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port (matches Program.cs)
EXPOSE 80
ENTRYPOINT ["dotnet", "OpenAIServer.dll"]
