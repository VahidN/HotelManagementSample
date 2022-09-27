using Microsoft.AspNetCore.Components;

namespace BlazorServer.App.Pages.Components;

public partial class Confirmation
{
    private bool ShowModal;

    [Parameter] public string Title { get; set; } = "Confirm";

    [Parameter] public string CancelButtonLabel { get; set; } = "Cancel";

    [Parameter] public string OkButtonLabel { get; set; } = "Ok";

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public EventCallback OnConfirm { get; set; }

    [Parameter] public EventCallback OnCancel { get; set; }

    public void Show() => ShowModal = true;

    public void Hide() => ShowModal = false;

    private async Task OnConfirmClicked()
    {
        ShowModal = false;
        await OnConfirm.InvokeAsync();
    }

    private async Task OnCancelClicked()
    {
        ShowModal = false;
        await OnCancel.InvokeAsync();
    }
}