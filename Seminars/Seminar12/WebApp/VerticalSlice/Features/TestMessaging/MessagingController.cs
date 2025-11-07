using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace VerticalSlice.Features.TestMessaging;

[ApiController]
[Route("api/messaging")]
public sealed class MessagingController : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(string text)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "seminar-queue",
            durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(text);

        await channel.BasicPublishAsync(string.Empty, "seminar-queue", false, body);

        return Ok($"Message '{text}' sent to queue.");
    }
}
