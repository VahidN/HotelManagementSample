using Microsoft.AspNetCore.Components;

namespace BlazorServer.App.Pages.Components;

public partial class DeleteConfirmation
{
    private bool IsProcessStart { get; set; }

    [Parameter] public EventCallback<bool> ConfirmationChanged { get; set; }

    [Parameter] public bool IsParentComponentProcessing { get; set; }

    protected override void OnParametersSet()
    {
        IsProcessStart = IsParentComponentProcessing;
    }

    private async Task OnConfirmationChange(bool value)
    {
        if (value)
        {
            IsProcessStart = true;
        }

        await ConfirmationChanged.InvokeAsync(value);
    }
}