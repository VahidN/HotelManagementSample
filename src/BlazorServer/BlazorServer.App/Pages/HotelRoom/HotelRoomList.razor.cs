using BlazorServer.App.Pages.Components;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.App.Pages.HotelRoom;

[Authorize(Roles = ConstantRoles.Admin)]
public partial class HotelRoomList
{
    private const string UploadFolder = "Uploads";
    private Confirmation Confirmation1 { set; get; } = default!;
    private List<HotelRoomDto> HotelRooms { set; get; } = new();
    private HotelRoomDto? RoomToBeDeleted { set; get; }

    [Inject] public IFileUploadService FileUploadService { set; get; } = default!;
    [Inject] public IWebHostEnvironment WebHostEnvironment { set; get; } = default!;

    private IHotelRoomService HotelRoomService => Service;

    private void OnCancelDeleteRoomClicked()
    {
        Confirmation1.Hide();
    }

    private void HandleDeleteRoom(HotelRoomDto roomDto)
    {
        RoomToBeDeleted = roomDto;
        Confirmation1.Show();
    }

    private async Task OnConfirmDeleteRoomClicked()
    {
        if (RoomToBeDeleted is null)
        {
            return;
        }

        await HotelRoomService.DeleteHotelRoomAsync(RoomToBeDeleted.Id);

        foreach (var imageDto in RoomToBeDeleted.HotelRoomImages)
        {
            var imageFileName =
                imageDto.RoomImageUrl?.Replace($"{UploadFolder}/", "", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                continue;
            }

            FileUploadService.DeleteFile(imageFileName, WebHostEnvironment.WebRootPath, UploadFolder);
        }

        HotelRooms.Remove(RoomToBeDeleted); // Update UI
    }

    protected override async Task OnInitializedAsync()
    {
        HotelRooms = await HotelRoomService.GetAllHotelRoomsAsync();
    }
}