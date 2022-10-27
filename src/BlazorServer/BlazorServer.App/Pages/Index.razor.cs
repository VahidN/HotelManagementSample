using BlazorServer.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.App.Pages;

public partial class Index
{
    private string? UserId { set; get; }

    [Inject] public IUserInfoService UserInfoService { set; get; } = default!;

    protected override async Task OnInitializedAsync()
    {
        UserId = await UserInfoService.GetUserIdAsync();
    }
}