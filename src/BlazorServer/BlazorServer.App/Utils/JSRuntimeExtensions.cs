using Microsoft.JSInterop;

namespace BlazorServer.App.Utils;

public static class JSRuntimeExtensions
{
    public static ValueTask ToastrSuccess(this IJSRuntime JSRuntime, string message) =>
        JSRuntime.InvokeVoidAsync("ShowToastr", "success", message);

    public static ValueTask ToastrError(this IJSRuntime JSRuntime, string message) =>
        JSRuntime.InvokeVoidAsync("ShowToastr", "error", message);
}