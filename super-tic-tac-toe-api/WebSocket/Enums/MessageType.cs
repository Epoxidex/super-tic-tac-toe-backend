namespace super_tic_tac_toe_api.WebSocket.Enums
{
    public enum MessageType
    {
        Joined = 101,
        Move = 102,

        IvalidParams = 200,
        LobbyNotFound = 201,
        PlayerAlreadyExist = 202,
        LobbyIsFull = 203,
        PlayerNotFound = 204,
        NotYourTurn = 205,
        BadMove = 206,
    }
}
