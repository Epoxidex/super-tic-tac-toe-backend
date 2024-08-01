using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    private static readonly Dictionary<int, List<WebSocket>> _sockets = new();

    public static async Task Handle(HttpContext context, WebSocket webSocket)
    {
        var query = context.Request.Query;
        if (!int.TryParse(query["lobbyId"], out int lobbyId))
        {
            context.Response.StatusCode = 400;
            return;
        }

        if (!_sockets.ContainsKey(lobbyId))
        {
            _sockets[lobbyId] = new List<WebSocket>();
        }

        _sockets[lobbyId].Add(webSocket);

        await ListenForMessages(webSocket, lobbyId);

        _sockets[lobbyId].Remove(webSocket);
    }

    private static async Task ListenForMessages(WebSocket webSocket, int lobbyId)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (webSocket.State == WebSocketState.Open)
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocket client", CancellationToken.None);
            }
        }
    }

    public static async Task SendMessageToLobby(int lobbyId, string message)
    {
        if (_sockets.ContainsKey(lobbyId))
        {
            var webSockets = _sockets[lobbyId];
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in webSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
