using System.Collections.Concurrent;
using IsLabApp.Models;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

<<<<<<< HEAD
// ============================================================
// Задание 2. Стандартный шаблонный эндпоинт /weatherforecast
// ============================================================
=======
>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

<<<<<<< HEAD
// ============================================================
// Задание 3. Диагностические эндпоинты /health и /version
// ============================================================

// GET /health -> { "status": "ok", "timestamp": "..." }
=======
//Диагностические эндпоинты /health и /version

>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow
    });
})
.WithName("GetHealth");

// GET /version -> { "name": "...", "version": "..." }
app.MapGet("/version", (IConfiguration config) =>
{
    var name = config["App:Name"] ?? "IsLabApp";
    var version = config["App:Version"] ?? "0.0.0";

    return Results.Ok(new
    {
        name,
        version
    });
})
.WithName("GetVersion");

<<<<<<< HEAD
// ============================================================
// Задание 4. Прикладной API "Заметки" (CRUD в памяти)
// ============================================================

// Хранилище в памяти. ConcurrentDictionary — чтобы безопасно работать
// с несколькими одновременными запросами.
=======
//Прикладной API "Заметки"

>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e
var notes = new ConcurrentDictionary<int, Note>();
var nextId = 0;

// POST /api/notes — создать заметку
app.MapPost("/api/notes", (CreateNoteDto dto) =>
{
    // Минимальная валидация входных данных
    if (string.IsNullOrWhiteSpace(dto.Title))
    {
        return Results.BadRequest(new { error = "Поле 'title' обязательно и не может быть пустым." });
    }

    if (dto.Title.Length > 200)
    {
        return Results.BadRequest(new { error = "Поле 'title' не может быть длиннее 200 символов." });
    }

    if (dto.Text is null)
    {
        return Results.BadRequest(new { error = "Поле 'text' обязательно." });
    }

    var id = Interlocked.Increment(ref nextId);
    var note = new Note
    {
        Id = id,
        Title = dto.Title.Trim(),
        Text = dto.Text,
        CreatedAt = DateTime.UtcNow
    };

    notes[id] = note;

    return Results.Created($"/api/notes/{id}", note);
})
.WithName("CreateNote");

// GET /api/notes — список всех заметок
app.MapGet("/api/notes", () =>
{
    var all = notes.Values.OrderBy(n => n.Id).ToList();
    return Results.Ok(all);
})
.WithName("GetNotes");

// GET /api/notes/{id} — получить одну заметку
app.MapGet("/api/notes/{id:int}", (int id) =>
{
    if (notes.TryGetValue(id, out var note))
    {
        return Results.Ok(note);
    }

    return Results.NotFound(new { error = $"Заметка с id={id} не найдена." });
})
.WithName("GetNoteById");

// DELETE /api/notes/{id} — удалить заметку
app.MapDelete("/api/notes/{id:int}", (int id) =>
{
    if (notes.TryRemove(id, out _))
    {
        return Results.NoContent();
    }

    return Results.NotFound(new { error = $"Заметка с id={id} не найдена." });
})
.WithName("DeleteNote");

<<<<<<< HEAD
// ============================================================
// Задание 5. Заготовка под MS SQL Server: /db/ping
// ============================================================
=======
// Заготовка под MS SQL Server: /db/ping
>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e

// GET /db/ping — пробует подключиться к БД по строке подключения
// ConnectionStrings:Mssql из appsettings.json. На этом этапе SQL Server
// ещё может быть не развёрнут, поэтому ошибка подключения — ожидаемый результат.
app.MapGet("/db/ping", async (IConfiguration config) =>
{
    var connectionString = config.GetConnectionString("Mssql");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Ok(new
        {
            status = "error",
            message = "Строка подключения ConnectionStrings:Mssql не задана в конфигурации."
        });
    }

    try
    {
        await using var connection = new SqlConnection(connectionString);

        // Не ждём вечно, если сервер недоступен
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await connection.OpenAsync(cts.Token);

        return Results.Ok(new
        {
            status = "ok",
            message = "Подключение к базе данных установлено."
        });
    }
    catch (Exception ex)
    {
        // Ожидаемо на этапе, пока SQL Server не развёрнут
        return Results.Ok(new
        {
            status = "error",
            message = ex.Message
        });
    }
})
.WithName("DbPing");

app.Run();

<<<<<<< HEAD
// Требуется для шаблонного /weatherforecast
=======
>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
<<<<<<< HEAD

// Делаем неявный класс Program публичным и частичным, чтобы тестовый
// проект мог подключить приложение через WebApplicationFactory<Program>
// (см. IsLabApp.Tests).
public partial class Program { }
=======
>>>>>>> 36c0bbd50cc97298911b4cdd42efce0bf361a54e
