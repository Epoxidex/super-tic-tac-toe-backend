using Microsoft.AspNetCore.Mvc;
using super_tic_tac_toe_logic.Enums;
using super_tic_tac_toe_logic.Extensions;

namespace super_tic_tac_toe_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbyController : ControllerBase
    {
        private static List<Lobby> lobbies = new List<Lobby>();

        [HttpPost("create")]
        public IActionResult CreateLobby()
        {
            var lobby = new Lobby();
            lobbies.Add(lobby);
            return Ok(new { LobbyCode = lobby.LobbyCode });
        }

        [HttpPost("join")]
        public IActionResult JoinLobby([FromBody] JoinLobbyRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            var playerType = lobby.Players.Count == 0 ? CellType.X : CellType.O;
            var player = new Player(request.PlayerName, playerType);

            if (!lobby.AddPlayer(player))
                return BadRequest("Lobby is full.");

            return Ok(new { PlayerType = player.PlayerType });
        }

        [HttpPost("start")]
        public IActionResult StartGame([FromBody] StartGameRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            if (!lobby.StartGame())
                return BadRequest("The game could not be started. There must be 2 players in the lobby.");

            return Ok("Game start.");
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == request.LobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found.");

            var player = lobby.Players.FirstOrDefault(p => p.Name == request.PlayerName);
            if (player == null)
                return NotFound("Player not found.");

            if (lobby.CurrentGame.CurrentPlayer != player.PlayerType)
                return BadRequest("It's not your turn now.");

            bool moveSuccessful = lobby.CurrentGame.MakeMove(request.SubGridRow, request.SubGridCol, request.CellRow, request.CellCol);

            if (!moveSuccessful)
                return BadRequest("Incorrect move.");

            return Ok("The move was completed successfully.");
        }

        [HttpGet("state")]
        public IActionResult GetGameState([FromQuery] int lobbyCode)
        {
            var lobby = lobbies.FirstOrDefault(l => l.LobbyCode == lobbyCode);
            if (lobby == null)
                return NotFound("lobby not found.");

            var gameState = new
            {
                MacroGrid = lobby.CurrentGame.MacroGrid,
                SubGrids = lobby.CurrentGame.SubGrids.ToEnumerable().Select(subGrid => subGrid.GetGridState()).ToArray(),
                CurrentPlayer = lobby.CurrentGame.CurrentPlayer,
                Winner = lobby.CurrentGame.Winner,
                MoveField = lobby.CurrentGame.MoveField
            };

            return Ok(gameState);
        }
    }

    public class JoinLobbyRequest
    {
        public int LobbyCode { get; set; }
        public string PlayerName { get; set; }
    }

    public class StartGameRequest
    {
        public int LobbyCode { get; set; }
    }

    public class MoveRequest
    {
        public int LobbyCode { get; set; }
        public string PlayerName { get; set; }
        public int SubGridRow { get; set; }
        public int SubGridCol { get; set; }
        public int CellRow { get; set; }
        public int CellCol { get; set; }
    }
}
