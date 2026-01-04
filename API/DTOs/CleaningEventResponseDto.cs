namespace CleanTogether.API.DTOs;

public class CleaningEventResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public TimeSpan EventTime { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int AttendeesCount { get; set; }
    public string ImageBase64 { get; set; } = null!;
}
