namespace VerticalSlice.Features.Wolverine;

public sealed record SeminarMessage(string Text);

public sealed class SeminarMessageHandler(ILogger<SeminarMessageHandler> logger)
{
    private readonly ILogger<SeminarMessageHandler> _logger = logger;

    public Task Handle(SeminarMessage message)
    {
        _logger.LogInformation("Received message through Wolverine: {Text}", message.Text);
        return Task.CompletedTask;
    }
}
