namespace super_tic_tac_toe_api.Connection.RequestsModels
{
    public class DeletePlayerRequest
    {
        public int LobbyId { get; set; }
        public string PlayerName { get; set; }
    }

    public class JoinLobbyRequest
    {
        public int LobbyId { get; set; }
        public string PlayerName { get; set; }
    }

    public class DeleteLobbyRequest
    {
        public int LobbyId { get; set; }
    }

    public class MoveRequest
    {
        public int LobbyId { get; set; }
        public string PlayerName { get; set; }
        public int SectorRow { get; set; }
        public int SectorCol { get; set; }
        public int CellRow { get; set; }
        public int CellCol { get; set; }
    }
}
