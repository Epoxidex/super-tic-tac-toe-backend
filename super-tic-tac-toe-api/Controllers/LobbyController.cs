using Microsoft.AspNetCore.Mvc;
using super_tic_tac_toe_api.Logic.Enums;
using super_tic_tac_toe_logic.Extensions;
using super_tic_tac_toe_api.Connection.RequestsModels;
using super_tic_tac_toe_api.Helpers;

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
            var lobby = new Lobby();
            lobbies.Add(lobby);
            return Ok(new { LobbyCode = lobby.LobbyCode });
        }

        [HttpPost("joinLobby")]
        public IActionResult JoinLobby([FromBody] JoinLobbyRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            if (lobby.Players.Where(p => p.Name == request.PlayerName).Any())
                return BadRequest($"Player '{request.PlayerName}' already exist.");
            var playerType = lobby.Players.Count == 0 ? CellType.X : CellType.O;
            var player = new Player(request.PlayerName, playerType);

            if (!lobby.AddPlayer(player))
                return BadRequest("Lobby is full.");

            return Ok(new { PlayerType = player.PlayerType });
        }

        [HttpPost("startGame")]
        public IActionResult StartGame([FromBody] StartGameRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            if (!lobby.StartGame())
                return BadRequest("The game could not be started. There must be 2 players in the lobby.");

            return Ok("Game start.");
        }

        [HttpPost("makeMove")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            var player = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (player == null)
                return NotFound("Player not found.");

            if (lobby.CurrentGame.Turn != player.PlayerType)
                return BadRequest("It's not your turn now.");

            bool moveSuccessful = lobby.CurrentGame.MakeMove(request.SectorRow, request.SectorCol, request.CellRow, request.CellCol);

            if (!moveSuccessful)
                return BadRequest("Incorrect move.");

            return Ok("The move was completed successfully.");
        }

        [HttpGet("getGameState")]
        public IActionResult GetGameState([FromQuery] int lobbyCode)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == lobbyCode);
            if (lobby == null)
                return NotFound("lobby not found.");

            var gameState = new
            {
                Board = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.Board),
                Sectors = lobby.CurrentGame.Sectors.ToEnumerable().Select(subGrid => ArrayHelper.ConvertToNestedLists(subGrid.Cells)).ToArray(),
                Turn = lobby.CurrentGame.Turn,
                Winner = lobby.CurrentGame.Winner,
                MoveField = ArrayHelper.ConvertToNestedLists(lobby.CurrentGame.MoveField)
            };

            return Ok(gameState);
        }

        [HttpDelete("deleteLobby")]
        public IActionResult DeleteLobby([FromBody] DeleteLobbyRequest request)
        {
            Lobby? lobbyToRemove = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobbyToRemove == null)
                return NotFound("Lobby not found.");
            lobbies.Remove(lobbyToRemove);
            return Ok($"Lobby {request.LobbyCode} removed.");
        }

        [HttpDelete("deletePlayer")]
        public IActionResult DeletePlayer([FromBody] DeletePlayerRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("lobby not found.");

            Player? playerToRemove = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (playerToRemove == null)
                return NotFound("Player not found");

            lobby.Players.Remove(playerToRemove);
            return Ok($"Player {request.PlayerName} removed.");
        }
    }
}
