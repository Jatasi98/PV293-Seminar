using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace VerticalSlice.Features.Wolverine;

[ApiController]
[Route("api/wolverine")]
public sealed class WolverineController(IMessageBus bus) : ControllerBase
{
    private readonly IMessageBus bus = bus;

    [HttpPost("send")]
    public async Task<IActionResult> SendWithWolverine(string text)
    {
        await bus.PublishAsync(new SeminarMessage(text));

        return Ok($"Sent Wolverine message: {text}");
    }
}
