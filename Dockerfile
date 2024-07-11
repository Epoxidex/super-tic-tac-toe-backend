# Stage 1: Build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["super-tic-tac-toe-api/super-tic-tac-toe-api.csproj", "super-tic-tac-toe-api/"]
RUN dotnet restore "./super-tic-tac-toe-api/super-tic-tac-toe-api.csproj"
COPY . .
WORKDIR "/src/super-tic-tac-toe-api"
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./super-tic-tac-toe-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "./super-tic-tac-toe-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "super-tic-tac-toe-api.dll"]
