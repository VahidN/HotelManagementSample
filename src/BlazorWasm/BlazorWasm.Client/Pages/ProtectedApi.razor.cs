using BlazorServer.Models;
using BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Client.Pages;

[Authorize]
public partial class ProtectedApi
{
    private ProtectedEditorsApiDto? Details { set; get; }

    [Inject] public IClientProtectedApiService ProtectedApiService { set; get; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Details = await ProtectedApiService.GetDetailsAsync();
    }
}