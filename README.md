# Super Tic-Tac-Toe API

This API provides functionality for a supercharged version of tic-tac-toe, allowing players to create lobbies, join games, make moves, and get game states.

## Getting Started

To get this bad boy up and running, you need [Docker](https://www.docker.com/) installed on your system.

### Prerequisites

Make sure you have Docker installed. That's non-negotiable.

### Installation

1. Clone this repository.
   ```bash
   git clone https://github.com/Epoxidex/super-tic-tac-toe-api.git
   cd super-tic-tac-toe-api
   ```

2. Build the Docker container.
   ```bash
   docker-compose up --build
   ```

3. The API will be served at `http://localhost:8080`.

## API Endpoints

### Creating a Lobby

**POST** `/api/lobby/createLobby`

Creates a new lobby for players to join.

### Joining a Lobby

**POST** `/api/lobby/joinLobby`

Joins an existing lobby with a specified lobby code.

### Starting the Game

**POST** `/api/lobby/startGame`

Starts the game in a lobby with the specified lobby code.

### Making a Move

**POST** `/api/lobby/makeMove`

Allows a player to make a move in the game.

### Getting Game State

**GET** `/api/lobby/getGameState?lobbyCode={lobbyCode}`

Returns the current state of the game in the specified lobby.

### Deleting a Lobby

**DELETE** `/api/lobby/deleteLobby`

Deletes a lobby with the specified lobby code.

### Deleting a Player

**DELETE** `/api/lobby/deletePlayer`

Removes a player from the lobby.

## Environment Variables

- `ASPNETCORE_HTTP_PORTS`: Port for HTTP traffic. Default is `8080`.
- `ASPNETCORE_ENVIRONMENT`: Environment mode. Default is `Development`.

## Docker Configuration

The API is Dockerized for ease of deployment. The `docker-compose.yml` file in the root directory defines the services required to run the API.

### Docker Commands

- Build and start containers:
  ```bash
  docker-compose up --build
  ```

- Stop containers:
  ```bash
  docker-compose down
  ```
