using BlazorServer.Models;
using BlazorWasm.Client.Utils;

namespace BlazorWasm.Client.Services;

public class ClientHotelRoomService : IClientHotelRoomService
{
    private readonly HttpClient _httpClient;

    public ClientHotelRoomService(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public Task<HotelRoomDto?> GetHotelRoomDetailsAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        // How to url-encode query-string parameters properly
        var httpClientBaseAddress = _httpClient.GetBaseAddressUri();
        var uri = new UriBuilderExt(new Uri(httpClientBaseAddress, Invariant($"/api/hotelroom/{roomId}")))
                  .AddParameter("checkInDate", Invariant($"{checkInDate:yyyy'-'MM'-'dd}"))
                  .AddParameter("checkOutDate", Invariant($"{checkOutDate:yyyy'-'MM'-'dd}"))
                  .Uri;
        return _httpClient.GetFromJsonAsync<HotelRoomDto>(uri);
    }

    public Task<IEnumerable<HotelRoomDto>?> GetHotelRoomsAsync(DateTime checkInDate, DateTime checkOutDate)
    {
        // How to url-encode query-string parameters properly
        var httpClientBaseAddress = _httpClient.GetBaseAddressUri();
        var uri = new UriBuilderExt(new Uri(httpClientBaseAddress, "/api/hotelroom"))
                  .AddParameter("checkInDate", Invariant($"{checkInDate:yyyy'-'MM'-'dd}"))
                  .AddParameter("checkOutDate", Invariant($"{checkOutDate:yyyy'-'MM'-'dd}"))
                  .Uri;
        return _httpClient.GetFromJsonAsync<IEnumerable<HotelRoomDto>>(uri);
    }
}