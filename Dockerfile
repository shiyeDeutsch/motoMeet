# Use the official .NET 8 SDK image as base
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Set the working directory inside the container
WORKDIR /src

# Copy project file(s) first to take advantage of Docker build cache
COPY *.csproj ./

# Restore dependencies (cached unless .csproj changes)
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Default command runs the app with hot reload
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:80"]
