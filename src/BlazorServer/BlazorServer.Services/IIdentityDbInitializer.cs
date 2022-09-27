namespace BlazorServer.Services;

public interface IIdentityDbInitializer
{
    Task SeedDatabaseWithAdminUserAsync();
}