using super_tic_tac_toe_api.Models;

namespace super_tic_tac_toe_api.Services.Interfaces
{
    public interface ILobbyService
    {
        string CreateLobby();
        string JoinLobby(JoinLobbyRequest request);
        Task<string> MakeMove(MoveRequest request);
        string GetGameState(int lobbyId);
        string GetLobbyState(int lobbyId);
        string DeleteLobby(DeleteLobbyRequest request);
        string DeletePlayer(DeletePlayerRequest request);
    }
}
