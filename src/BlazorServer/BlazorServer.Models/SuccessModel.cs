namespace BlazorServer.Models;

public class SuccessModel
{
    public string? Title { get; set; }

    public int StatusCode { get; set; }

    public string? SuccessMessage { get; set; }

    public object? Data { get; set; }
}