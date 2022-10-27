namespace BlazorServer.Services;

public interface IUserInfoService
{
    Task<string?> GetUserIdAsync();
}