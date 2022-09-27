using BlazorServer.Entities;

namespace BlazorServer.Services;

public interface ITokenFactoryService
{
    Task<string> CreateJwtTokensAsync(ApplicationUser user);
}