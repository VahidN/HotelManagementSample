using Blazored.TextEditor;
using BlazorServer.App.Pages.Components;
using BlazorServer.App.Utils;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorServer.App.Pages.HotelRoom;

public partial class HotelRoomUpsert
{
    private const string UploadFolder = "Uploads";

    private List<string> DeletedImageFileNames { get; } = new();
    private Confirmation Confirmation1 { set; get; } = default!;
    private HotelRoomDto? HotelRoomModel { set; get; }
    private HotelRoomImageDto? ImageToBeDeleted { set; get; }
    private bool IsImageUploadProcessStarted { set; get; }
    private BlazoredTextEditor QuillHtml { set; get; } = default!;
    private string Title { set; get; } = "Create";

    [Inject] public NavigationManager NavigationManager { set; get; } = default!;
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;
    [Inject] public IFileUploadService FileUploadService { set; get; } = default!;
    [Inject] public IWebHostEnvironment WebHostEnvironment { set; get; } = default!;

    private IHotelRoomService HotelRoomService => Service;

    [CascadingParameter] public Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter] public int? Id { get; set; }

    private void OnCancelDeleteImageClicked()
    {
        Confirmation1.Hide();
    }

    private void DeletePhoto(HotelRoomImageDto imageDto)
    {
        ImageToBeDeleted = imageDto;
        Confirmation1.Show();
    }

    private async Task OnConfirmDeleteImageClicked(HotelRoomImageDto? imageDto)
    {
        if (imageDto is null)
        {
            return;
        }

        var imageFileName = imageDto.RoomImageUrl?.Replace($"{UploadFolder}/", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(imageFileName))
        {
            await JsRuntime.ToastrError("imageFileName is null");
            return;
        }

        if (HotelRoomModel is null)
        {
            await JsRuntime.ToastrError("HotelRoomModel is null");
            return;
        }

        if (HotelRoomModel.Id == 0 && string.Equals(Title, "Create", StringComparison.OrdinalIgnoreCase))
        {
            // Create Mode
            FileUploadService.DeleteFile(imageFileName, WebHostEnvironment.WebRootPath, UploadFolder);
            HotelRoomModel.HotelRoomImages.Remove(imageDto); // Update UI
        }
        else
        {
            // Edit Mode
            DeletedImageFileNames.Add(imageFileName);
            HotelRoomModel.HotelRoomImages.Remove(imageDto); // Update UI
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is null)
        {
            // In a BlazorServer component, JSRuntime is not available in OnInitialize or OnInitializedAsync.
            // await JsRuntime.ToastrError("AuthenticationState is null")
            return;
        }

        var authenticationState = await AuthenticationState;
        if (authenticationState.User.Identity?.IsAuthenticated == false ||
            !authenticationState.User.IsInRole(ConstantRoles.Admin))
        {
            var uri = new Uri(NavigationManager.Uri);
            NavigationManager.NavigateTo($"/identity/account/login?returnUrl={uri.LocalPath}");
        }

        // authenticationState.User.IsInRole()

        if (Id.HasValue)
        {
            // Update Mode
            Title = "Update";
            HotelRoomModel = await HotelRoomService.GetHotelRoomAsync(Id.Value);
        }
        else
        {
            // Create Mode
            Title = "Create";
            HotelRoomModel = new HotelRoomDto();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        while (true)
        {
            try
            {
                await SetHTMLAsync();
                break;
            }
            catch
            {
                await Task.Delay(100); // Quill needs some time to load
            }
        }
    }

    private async Task HandleHotelRoomUpsert()
    {
        if (HotelRoomModel is null)
        {
            await JsRuntime.ToastrError("HotelRoomModel is null");
            return;
        }

        var isRoomUnique = await HotelRoomService.IsRoomUniqueAsync(HotelRoomModel.Name, HotelRoomModel.Id);
        if (!isRoomUnique)
        {
            await JsRuntime.ToastrError($"The room name: `{HotelRoomModel.Name}` already exists.");
            return;
        }

        if (HotelRoomModel.Id != 0 && string.Equals(Title, "Update", StringComparison.Ordinal))
        {
            // Update Mode
            HotelRoomModel.Details = await QuillHtml.GetHTML();
            await HotelRoomService.UpdateHotelRoomAsync(HotelRoomModel);

            foreach (var imageFileName in DeletedImageFileNames)
            {
                FileUploadService.DeleteFile(imageFileName, WebHostEnvironment.WebRootPath, UploadFolder);
            }

            await JsRuntime.ToastrSuccess($"The `{HotelRoomModel.Name}` updated successfully.");
        }
        else
        {
            // Create Mode
            HotelRoomModel.Details = await QuillHtml.GetHTML();
            await HotelRoomService.CreateHotelRoomAsync(HotelRoomModel);
            await JsRuntime.ToastrSuccess($"The `{HotelRoomModel.Name}` created successfully.");
        }

        NavigationManager.NavigateTo("hotel-room");
    }

    private async Task HandleImageUpload(InputFileChangeEventArgs args)
    {
        try
        {
            IsImageUploadProcessStarted = true;

            if (HotelRoomModel is null)
            {
                await JsRuntime.ToastrError("HotelRoomModel is null");
                return;
            }

            var files = args.GetMultipleFiles(5);
            if (args.FileCount == 0 || files.Count == 0)
            {
                return;
            }

            var allowedExtensions = new List<string> { ".jpg", ".png", ".jpeg" };
            if (!files.Any(file => allowedExtensions.Contains(Path.GetExtension(file.Name),
                                                              StringComparer.OrdinalIgnoreCase)))
            {
                await JsRuntime.ToastrError("Please select .jpg/.jpeg/.png files only.");
                return;
            }

            foreach (var file in files)
            {
                var uploadedImageUrl =
                    await FileUploadService.UploadFileAsync(file, WebHostEnvironment.WebRootPath, UploadFolder);
                HotelRoomModel.HotelRoomImages.Add(new HotelRoomImageDto { RoomImageUrl = uploadedImageUrl });
            }
        }
        finally
        {
            IsImageUploadProcessStarted = false;
        }
    }

    private async Task SetHTMLAsync()
    {
        if (!string.IsNullOrEmpty(HotelRoomModel?.Details))
        {
            await QuillHtml.LoadHTMLContent(HotelRoomModel.Details);
            StateHasChanged();
        }
    }
}