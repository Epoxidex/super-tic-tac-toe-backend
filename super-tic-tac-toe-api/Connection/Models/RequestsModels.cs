namespace super_tic_tac_toe_api.Connection.RequestsModels
{
    public class DeletePlayerRequest
    {
        public int LobbyCode { get; set; }
        public string PlayerName { get; set; }
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

    public class DeleteLobbyRequest
    {
        public int LobbyCode { get; set; }
    }

    public class MoveRequest
    {
        public int LobbyCode { get; set; }
        public string PlayerName { get; set; }
        public int SectorRow { get; set; }
        public int SectorCol { get; set; }
        public int CellRow { get; set; }
        public int CellCol { get; set; }
    }
}
