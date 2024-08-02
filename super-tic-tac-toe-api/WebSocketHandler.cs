using Newtonsoft.Json;
using super_tic_tac_toe_api.Models;
using super_tic_tac_toe_api.Services.Interfaces;
using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    private static readonly Dictionary<int, Dictionary<string, WebSocket>> _sockets = new();
    private static ILobbyService _lobbyService;
    public static void Initialize(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }
    public static async Task Handle(HttpContext context, WebSocket webSocket)
    {
        var query = context.Request.Query;
        if (!int.TryParse(query["lobbyId"], out int lobbyId) || string.IsNullOrEmpty(query["playerName"]))
        {
            return;
        }

        if (!_lobbyService.HasLobby(lobbyId))
        {
            return;
        }

        if (!_sockets.ContainsKey(lobbyId))
        {
            _sockets[lobbyId] = new Dictionary<string, WebSocket>();
        }

        string playerName = query["playerName"];

        var joinResponse = _lobbyService.JoinLobby(new JoinLobbyRequest { LobbyId = lobbyId, PlayerName = playerName });
        if (joinResponse.Contains("error"))
        {
            return;
        }

        _sockets[lobbyId][playerName] = webSocket;

        await ListenForMessages(webSocket, lobbyId, playerName);

        _sockets[lobbyId].Remove(playerName);
        _lobbyService.DeletePlayer(new DeletePlayerRequest { LobbyId = lobbyId, PlayerName=playerName });

        //TODO add lobby removing
        //TODO removing player from lobby
    }

    private static async Task ListenForMessages(WebSocket webSocket, int lobbyId, string playerName)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (webSocket.State == WebSocketState.Open)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await ProcessMessage(lobbyId, playerName, message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocket client", CancellationToken.None);
            }
        }
    }

    private static async Task ProcessMessage(int lobbyId, string playerName, string message)
    {
        var moveRequest = JsonConvert.DeserializeObject<MoveRequest>(message);
        if (moveRequest != null)
        {
            var response = await _lobbyService.MakeMove(moveRequest);
            if (response.Contains("error"))
            {
                await SendMessageToPlayer(lobbyId, playerName, response);
            }
            else
            {
                var secondPlayer = _sockets[lobbyId].Where(p => p.Key != playerName).FirstOrDefault().Key;
                var gameState = _lobbyService.GetGameState(lobbyId);
                await SendMessageToPlayer(lobbyId, secondPlayer, gameState);
            }
        }
    }

    public static async Task SendMessageToPlayer(int lobbyId, string playerName, string message)
    {
        if (_sockets.ContainsKey(lobbyId) && _sockets[lobbyId].ContainsKey(playerName))
        {
            var webSocket = _sockets[lobbyId][playerName];
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public static async Task SendMessageToLobby(int lobbyId, string message)
    {
        if (_sockets.ContainsKey(lobbyId))
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _sockets[lobbyId].Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
