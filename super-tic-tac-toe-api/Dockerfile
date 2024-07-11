#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["super-tic-tac-toe-api/super-tic-tac-toe-api.csproj", "super-tic-tac-toe-api/"]
COPY ["super-tic-tac-toe-logic/super-tic-tac-toe-logic.csproj", "super-tic-tac-toe-logic/"]
RUN dotnet restore "./super-tic-tac-toe-api/super-tic-tac-toe-api.csproj"
COPY . .
WORKDIR "/src/super-tic-tac-toe-api"
RUN dotnet build "./super-tic-tac-toe-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./super-tic-tac-toe-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "super-tic-tac-toe-api.dll"]