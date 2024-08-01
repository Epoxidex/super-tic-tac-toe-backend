using Newtonsoft.Json;
using Serilog;
using super_tic_tac_toe_api.Entities;
using super_tic_tac_toe_api.Helpers;
using super_tic_tac_toe_api.Logic.Enums;
using super_tic_tac_toe_api.Models;
using super_tic_tac_toe_api.Services.Interfaces;

namespace super_tic_tac_toe_api.Services
{
    public class LobbyService : ILobbyService
    {
        private static List<Lobby> lobbies = new List<Lobby>();

        public string CreateLobby()
        {
            Log.Information("Creating a new lobby");

            var lobby = new Lobby();
            lobbies.Add(lobby);

            Log.Information("Lobby created with ID {LobbyId}", lobby.LobbyId);
            return JsonConvert.SerializeObject(new { lobbyId = lobby.LobbyId }, Formatting.Indented);
        }

        public string JoinLobby(JoinLobbyRequest request)
        {
            Log.Information("Player {PlayerName} is attempting to join lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {   
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            if (lobby.Players.Where(p => p.Name == request.PlayerName).Any())
            {
                Log.Warning("Player {PlayerName} already exists in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return JsonConvert.SerializeObject(new { error = $"Player '{request.PlayerName}' already exist." }, Formatting.Indented);
            }

            CellType playerType = lobby.Players.Count switch
            {
                0 => CellType.X,
                1 => CellType.O,
                _ => CellType.None,
            };

            var player = new Player(request.PlayerName, playerType);

            if (!lobby.AddPlayer(player))
            {
                Log.Warning("Lobby {LobbyId} is full", request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby is full." }, Formatting.Indented);
            }

            Log.Information("Player {PlayerName} joined lobby {LobbyId} as {PlayerType}", request.PlayerName, request.LobbyId, player.PlayerType);
            return JsonConvert.SerializeObject(new { playerType = player.PlayerType }, Formatting.Indented);
        }

        public string MakeMove(MoveRequest request)
        {
            Log.Information("Player {PlayerName} is making a move in lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            var player = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (player == null)
            {
                Log.Warning("Player {PlayerName} not found in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Player not found." }, Formatting.Indented);
            }

            if (lobby.CurrentGame.Turn != player.PlayerType)
            {
                Log.Warning("It's not {PlayerName}'s turn in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "It's not your turn now." }, Formatting.Indented);
            }

            bool moveSuccessful = lobby.CurrentGame.MakeMove(request.SectorRow, request.SectorCol, request.CellRow, request.CellCol);

            if (!moveSuccessful)
            {
                Log.Warning("Invalid move by {PlayerName} in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Incorrect move." }, Formatting.Indented);
            }

            Log.Information("Move by {PlayerName} completed successfully in lobby {LobbyId}", request.PlayerName, request.LobbyId);
            return JsonConvert.SerializeObject(new { success = "The move was completed successfully." }, Formatting.Indented);
        }

        public string GetGameState(int lobbyId)
        {
            Log.Information("Getting game state for lobby {LobbyId}", lobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == lobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", lobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            var gameState = new
            {
                board = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.Board),
                sectors = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.Sectors).Select(x => x.Select(y => ArrayHelper.ConvertToNestedLists(y.Board))),
                turn = lobby.CurrentGame.Turn,
                winner = lobby.CurrentGame.Winner,
                openSectors = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.OpenSectors)
            };

            Log.Information("Game state retrieved for lobby {LobbyId}", lobbyId);
            return JsonConvert.SerializeObject(gameState, Formatting.Indented);
        }

        public string GetLobbyState(int lobbyId)
        {
            Log.Information("Getting lobby state for lobby {LobbyId}", lobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == lobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", lobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            var playerStates = lobby.Players.Select(p => new Dictionary<string, CellType> { { p.Name, p.PlayerType } }).ToList();

            Log.Information("Lobby state retrieved for lobby {LobbyId}", lobbyId);
            return JsonConvert.SerializeObject(playerStates, Formatting.Indented);
        }

        public string DeleteLobby(DeleteLobbyRequest request)
        {
            Log.Information("Deleting lobby {LobbyId}", request.LobbyId);

            Lobby? lobbyToRemove = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobbyToRemove == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            lobbies.Remove(lobbyToRemove);

            Log.Information("Lobby {LobbyId} removed", request.LobbyId);
            return JsonConvert.SerializeObject(new { success = $"Lobby {request.LobbyId} removed." }, Formatting.Indented);
        }

        public string DeletePlayer(DeletePlayerRequest request)
        {
            Log.Information("Deleting player {PlayerName} from lobby {LobbyId}", request.PlayerName, request.LobbyId);

            var lobby = lobbies.FirstOrDefault(l => l.LobbyId == request.LobbyId);
            if (lobby == null)
            {
                Log.Warning("Lobby {LobbyId} not found", request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Lobby not found." }, Formatting.Indented);
            }

            Player? playerToRemove = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (playerToRemove == null)
            {
                Log.Warning("Player {PlayerName} not found in lobby {LobbyId}", request.PlayerName, request.LobbyId);
                return JsonConvert.SerializeObject(new { error = "Player not found." }, Formatting.Indented);
            }

            lobby.Players.Remove(playerToRemove);

            Log.Information("Player {PlayerName} removed from lobby {LobbyId}", request.PlayerName, request.LobbyId);
            return JsonConvert.SerializeObject(new { success = $"Player {request.PlayerName} removed from lobby {request.LobbyId}." }, Formatting.Indented);
        }
    }
}
