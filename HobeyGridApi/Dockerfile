FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src/HobeyGridApi

# Copy the project file and restore dependencies. This is optimized for caching.
COPY ["HobeyGridApi.csproj", "./"]
RUN dotnet restore "HobeyGridApi.csproj"

# Copy the rest of the application files
COPY . .

# Build the application, specifying the x64 architecture for Azure App Service
RUN dotnet build "HobeyGridApi.csproj" -c Release -o /app/build --arch x64

# Stage 2: Create the final, lightweight runtime image
# Use the official .NET runtime image, based on Alpine for a small footprint
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Copy the compiled application and its dependencies from the 'build' stage
COPY --from=build /app/build .

# Expose the port that the ASP.NET Core application listens on
EXPOSE 8080

# Set the entry point to run the application's DLL
ENTRYPOINT ["dotnet", "HobeyGridApi.dll"]