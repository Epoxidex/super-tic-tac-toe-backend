using Microsoft.AspNetCore.Mvc;
using super_tic_tac_toe_api.Models;
using super_tic_tac_toe_api.Services.Interfaces;

namespace super_tic_tac_toe_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbyController : ControllerBase
    {
        private readonly ILobbyService _lobbyService;
        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        [HttpPost("createLobby")]
        public IActionResult CreateLobby()
        {
            var result = _lobbyService.CreateLobby();
            return Ok(result);
        }

        [HttpPost("joinLobby")]
        public IActionResult JoinLobby([FromBody] JoinLobbyRequest request)
        {
            var result = _lobbyService.JoinLobby(request);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("makeMove")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            var result = _lobbyService.MakeMove(request);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getGameState")]
        public IActionResult GetGameState([FromQuery] int lobbyId)
        {
            var result = _lobbyService.GetGameState(lobbyId);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getLobbyState")]
        public IActionResult GetLobbyState([FromQuery] int lobbyId)
        {
            var result = _lobbyService.GetLobbyState(lobbyId);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpDelete("deleteLobby")]
        public IActionResult DeleteLobby([FromBody] DeleteLobbyRequest request)
        {
            var result = _lobbyService.DeleteLobby(request);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("deletePlayer")]
        public IActionResult DeletePlayer([FromBody] DeletePlayerRequest request)
        {
            var result = _lobbyService.DeletePlayer(request);
            if (result.Contains("Error"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
