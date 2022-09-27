namespace BlazorWasm.Client.Models.ViewModels;

public class HomeVM
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; }

    public int NoOfNights { get; set; } = 1;
}