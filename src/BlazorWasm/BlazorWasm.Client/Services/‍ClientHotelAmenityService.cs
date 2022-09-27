using BlazorServer.Models;
using BlazorWasm.Client.Utils;

namespace BlazorWasm.Client.Services;

public class ClientHotelAmenityService : IClientHotelAmenityService
{
    private readonly HttpClient _httpClient;

    public ClientHotelAmenityService(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public Task<IEnumerable<HotelAmenityDto>?> GetHotelAmenities()
    {
        // How to url-encode query-string parameters properly
        var httpClientBaseAddress = _httpClient.GetBaseAddressUri();
        var uri = new UriBuilderExt(new Uri(httpClientBaseAddress, "/api/hotelamenity")).Uri;
        return _httpClient.GetFromJsonAsync<IEnumerable<HotelAmenityDto>>(uri);
    }
}