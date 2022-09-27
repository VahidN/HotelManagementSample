namespace BlazorServer.Models;

public class ErrorModel
{
    public string? Title { get; set; }

    public int StatusCode { get; set; }

    public string? ErrorMessage { get; set; }
}