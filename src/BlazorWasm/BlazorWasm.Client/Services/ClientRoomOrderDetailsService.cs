using BlazorServer.Models;
using BlazorWasm.Client.Utils;

namespace BlazorWasm.Client.Services;

public class ClientRoomOrderDetailsService : IClientRoomOrderDetailsService
{
    private readonly HttpClient _httpClient;

    public ClientRoomOrderDetailsService(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public Task<RoomOrderDetailsDto?> GetOrderDetailAsync(long trackingNumber)
    {
        // How to url-encode query-string parameters properly
        var httpClientBaseAddress = _httpClient.GetBaseAddressUri();
        var uri = new UriBuilderExt(new Uri(httpClientBaseAddress, "/api/roomorder/GetOrderDetail"))
                  .AddParameter("trackingNumber", trackingNumber.ToString(CultureInfo.InvariantCulture))
                  .Uri;
        return _httpClient.GetFromJsonAsync<RoomOrderDetailsDto>(uri);
    }

    public async Task<RoomOrderDetailsDto?> SaveRoomOrderDetailsAsync(RoomOrderDetailsDto details)
    {
        var response = await _httpClient.PostAsJsonAsync("api/roomorder/create", details);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<RoomOrderDetailsDto>(responseContent);
        }

        throw new InvalidOperationException(responseContent);
    }
}