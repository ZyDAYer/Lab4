using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IsLabApp.Tests;

// Простейшие тесты, проверяющие ожидаемое значение в ответах API.
// WebApplicationFactory<Program> поднимает приложение целиком in-memory
// (без реального хостинга и без реальной БД), поэтому подходит для
// быстрого шага "test" в CI-пайплайне.
public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsSuccessAndStatusOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);

        var status = json.RootElement.GetProperty("status").GetString();
        Assert.Equal("ok", status);
    }

    [Fact]
    public async Task GetVersion_ReturnsConfiguredAppName()
    {
        // Act
        var response = await _client.GetAsync("/version");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);

        // Значение берётся из appsettings.json: App:Name = "IsLabApp"
        var name = json.RootElement.GetProperty("name").GetString();
        Assert.Equal("IsLabApp", name);
    }
}
