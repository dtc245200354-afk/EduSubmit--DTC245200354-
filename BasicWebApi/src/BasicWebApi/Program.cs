
var builder = WebApplication.CreateBuilder(args);

// Dang ky dich vu
builder.Services.AddSingleton<GreetingService>();

var app = builder.Build();

// GET /
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        application = "BasicWebApi",
        status = "running"
    });
});

// GET /hello/{name}?language=...
app.MapGet("/hello/{name}", (string name, string? language) =>
{
    var message = language?.ToLowerInvariant() switch
    {
        "en" => $"Hello, {name}!",
        "ja" => $"Konnichiwa, {name}!",
        _ => $"Xin chào, {name}!"
    };

    return Results.Ok(new { message });
});

// POST /greetings
app.MapPost("/greetings",
    (CreateGreetingRequest request, GreetingService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new
        {
            message = "Name must not be empty."
        });
    }

    var greeting = service.Create(request.Name);

    return Results.Created(
        $"/greetings/{greeting.Id}",
        greeting
    );
});

app.Run();

// DTO
record CreateGreetingRequest(string Name);

// Model
record Greeting(int Id, string Name, DateTimeOffset CreatedAt);

// Service
class GreetingService
{
    private int _nextId = 0;

    public Greeting Create(string name)
    {
        return new Greeting(
            Interlocked.Increment(ref _nextId),
            name,
            DateTimeOffset.UtcNow
        );
    }
}
