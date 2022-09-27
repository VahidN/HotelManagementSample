using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public interface IClientHotelRoomService
{
    public Task<IEnumerable<HotelRoomDto>?> GetHotelRoomsAsync(DateTime checkInDate, DateTime checkOutDate);
    public Task<HotelRoomDto?> GetHotelRoomDetailsAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);
}