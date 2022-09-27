using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public interface IClientProtectedApiService
{
    Task<ProtectedEditorsApiDto?> GetDetailsAsync();
}