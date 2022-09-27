namespace BlazorWasm.Client.Utils;

public static class UriExt
{
    public static Uri GetBaseAddressUri(this HttpClient? httpClient)
    {
        var httpClientBaseAddress = httpClient?.BaseAddress;
        if (httpClientBaseAddress is null)
        {
            throw new InvalidOperationException("httpClientBaseAddress is null");
        }

        return httpClientBaseAddress;
    }
}