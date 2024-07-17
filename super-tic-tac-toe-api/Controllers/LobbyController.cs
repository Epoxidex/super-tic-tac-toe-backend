﻿using Microsoft.AspNetCore.Mvc;
using super_tic_tac_toe_api.Logic.Enums;
using super_tic_tac_toe_api.Connection.RequestsModels;
using super_tic_tac_toe_api.Helpers;
using Serilog;

namespace super_tic_tac_toe_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbyController : ControllerBase
    {
        private static List<Lobby> lobbies = new List<Lobby>();

        [HttpPost("createLobby")]
        public IActionResult CreateLobby()
        {
            Log.Information("Creating a new lobby");

            var lobby = new Lobby();
            lobbies.Add(lobby);

            Log.Information("Lobby created with ID {LobbyId}", lobby.LobbyId);
            return Ok(new { LobbyId = lobby.LobbyId });
        }

        [HttpPost("joinLobby")]
        public IActionResult JoinLobby([FromBody] JoinLobbyRequest request)
        {
            Log.Information("Player {PlayerName} is attempting to join lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return NotFound("Lobby not found.");
            }

            if (lobby.Players.Where(p => p.Name == request.PlayerName).Any())
            {
                Log.Warning("Player {PlayerName} already exists in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return BadRequest($"Player '{request.PlayerName}' already exist.");
            }

            var playerType = lobby.Players.Count == 0 ? CellType.X : CellType.O;
            var player = new Player(request.PlayerName, playerType);

            if (!lobby.AddPlayer(player))
            {
                Log.Warning("Lobby {LobbyId} is full", request.LobbyId);
                return BadRequest("Lobby is full.");
            }

            Log.Information("Player {PlayerName} joined lobby {LobbyId} as {PlayerType}", request.PlayerName, request.LobbyId, player.PlayerType);
            return Ok(new { PlayerType = player.PlayerType });
        }

        [HttpPost("startGame")]
        public IActionResult StartGame([FromBody] StartGameRequest request)
        {
            Log.Information("Starting game in lobby {LobbyId}", request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return NotFound("Lobby not found.");
            }

            if (!lobby.StartGame())
            {
                Log.Warning("Game could not be started in lobby {LobbyId}. Not enough players.", request.LobbyId);
                return BadRequest("The game could not be started. There must be 2 players in the lobby.");
            }

            Log.Information("Game started in lobby {LobbyId}", request.LobbyId);
            return Ok("Game start.");
        }

        [HttpPost("makeMove")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            Log.Information("Player {PlayerName} is making a move in lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return NotFound("Lobby not found.");
            }

            if (lobby.CurrentGame == null)
            {
                Log.Warning("Game not started in lobby {LobbyId}", request.LobbyId);
                return NotFound("Game not started.");
            }

            var player = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (player == null)
            {
                Log.Warning("Player {PlayerName} not found in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return NotFound("Player not found.");
            }

            if (lobby.CurrentGame.Turn != player.PlayerType)
            {
                Log.Warning("It's not {PlayerName}'s turn in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return BadRequest("It's not your turn now.");
            }

            bool moveSuccessful = lobby.CurrentGame.MakeMove(request.SectorRow, request.SectorCol, request.CellRow, request.CellCol);

            if (!moveSuccessful)
            {
                Log.Warning("Invalid move by {PlayerName} in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return BadRequest("Incorrect move.");
            }

            Log.Information("Move by {PlayerName} completed successfully in lobby {LobbyId}", request.PlayerName, request.LobbyId);
            return Ok("The move was completed successfully.");
        }

        [HttpGet("getGameState")]
        public IActionResult GetGameState([FromQuery] int lobbyId, [FromQuery] string playerName)
        {
            Log.Information("Getting game state for player {PlayerName} in lobby {LobbyId}", playerName, lobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == lobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", lobbyId);
                return NotFound("lobby not found.");
            }

            if (lobby.CurrentGame == null)
            {
                Log.Warning("Game not started in lobby {LobbyId}", lobbyId);
                return NotFound("Game not started.");
            }

            var player = lobby.Players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                Log.Warning("Player {PlayerName} not found in lobby {LobbyId}", playerName, lobbyId);
                return NotFound("Player not found.");
            }

            var gameState = new
            {
                Player = lobby.Players.Where(p => p.Name == playerName).Select(p => p.PlayerType).FirstOrDefault(),
                Board = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.Board),
                Sectors = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.Sectors).Select(x => x.Select(y => ArrayHelper.ConvertToNestedLists(y.Cells))),
                Turn = lobby.CurrentGame.Turn,
                Winner = lobby.CurrentGame.Winner,
                OpenSectors = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.OpenSectors)
            };

            Log.Information("Game state retrieved for player {PlayerName} in lobby {LobbyId}", playerName, lobbyId);
            return Ok(gameState);
        }

        [HttpDelete("deleteLobby")]
        public IActionResult DeleteLobby([FromBody] DeleteLobbyRequest request)
        {
            Log.Information("Deleting lobby {LobbyId}", request.LobbyId);

            Lobby? lobbyToRemove = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobbyToRemove == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return NotFound("Lobby not found.");
            }

            lobbies.Remove(lobbyToRemove);

            Log.Information("Lobby {LobbyId} removed", request.LobbyId);
            return Ok($"Lobby {request.LobbyId} removed.");
        }

        [HttpDelete("deletePlayer")]
        public IActionResult DeletePlayer([FromBody] DeletePlayerRequest request)
        {
            Log.Information("Deleting player {PlayerName} from lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return NotFound("lobby not found.");
            }

            Player? playerToRemove = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (playerToRemove == null)
            {
                Log.Warning("Player {PlayerName} not found in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return NotFound("Player not found");
            }

            lobby.Players.Remove(playerToRemove);

            Log.Information("Player {PlayerName} removed from lobby {LobbyId}", request.PlayerName, request.LobbyId);
            return Ok($"Player {request.PlayerName} removed from lobby {request.LobbyId}.");
        }
    }
}
