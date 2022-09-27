using BlazorServer.Models;

namespace BlazorServer.Services;

public interface IHotelRoomService : IDisposable
{
    Task<HotelRoomDto> CreateHotelRoomAsync(HotelRoomDto hotelRoomDto);

    Task<int> DeleteHotelRoomAsync(int roomId);

    Task<List<HotelRoomDto>> GetAllHotelRoomsAsync(DateTime? checkInDate = null, DateTime? checkOutDate = null);

    Task<HotelRoomDto?> GetHotelRoomAsync(int roomId, DateTime? checkInDate = null, DateTime? checkOutDate = null);

    Task<bool> IsRoomUniqueAsync(string name, int roomId);

    Task<HotelRoomDto?> UpdateHotelRoomAsync(HotelRoomDto hotelRoomDto);
}