using System.Net.WebSockets;
using System.Text;

Console.Title = "Client";

using var ws = new ClientWebSocket();

await ws.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
byte[] buf = new byte[1056];

while (ws.State == WebSocketState.Open)
{
    var result = await ws.ReceiveAsync(buf, CancellationToken.None);
    string? msg = string.Empty;

    if (result.MessageType == WebSocketMessageType.Close)
    {
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        Console.WriteLine(result.CloseStatusDescription);
    }
    else
    {
        Console.WriteLine(Encoding.ASCII.GetString(buf, 0, result.Count));
        msg = Console.ReadLine();
        if (string.IsNullOrEmpty(msg))
        {
            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
        else
        {
            byte[] data = Encoding.ASCII.GetBytes(msg!);
            await ws.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}