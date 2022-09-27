using BlazorServer.Models;

namespace BlazorServer.Services;

public interface IHotelRoomImageService : IDisposable
{
    Task<int> CreateHotelRoomImageAsync(HotelRoomImageDto imageDto);

    Task<int> DeleteHotelRoomImageByImageIdAsync(int imageId);

    Task<int> DeleteHotelRoomImageByRoomIdAsync(int roomId);

    Task<List<HotelRoomImageDto>> GetHotelRoomImagesAsync(int roomId);
}