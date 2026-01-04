namespace CleanTogether.API.DTOs;

public class CreateCleaningEventDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public TimeSpan EventTime { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public IFormFile Image { get; set; } = null!;
}
