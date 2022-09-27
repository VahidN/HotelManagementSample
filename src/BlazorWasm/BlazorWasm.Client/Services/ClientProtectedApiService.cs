using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public class ClientProtectedApiService : IClientProtectedApiService
{
    private readonly HttpClient _httpClient;

    public ClientProtectedApiService(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public Task<ProtectedEditorsApiDto?> GetDetailsAsync() =>
        _httpClient.GetFromJsonAsync<ProtectedEditorsApiDto>("/api/MyProtectedEditorsApi");
}