using BlazorServer.Models;

namespace BlazorServer.Services;

public interface IAmenityService : IDisposable
{
    Task<HotelAmenityDto> CreateHotelAmenityAsync(HotelAmenityDto hotelAmenity);

    Task<int> DeleteHotelAmenityAsync(int amenityId, string userId);

    Task<List<HotelAmenityDto>> GetAllHotelAmenityAsync();

    Task<HotelAmenityDto?> GetHotelAmenityAsync(int amenityId);

    Task<HotelAmenityDto?> IsSameNameAmenityAlreadyExistsAsync(string name);

    Task<HotelAmenityDto> UpdateHotelAmenityAsync(int amenityId, HotelAmenityDto hotelAmenity);
}