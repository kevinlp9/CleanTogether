namespace CleanTogether.API.Entities;

public class CleaningEvent
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime EventDate { get; set; }
    public TimeSpan EventTime { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Imagen en BD
    public byte[] Image { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    public ICollection<EventAttendee> EventAttendees { get; set; } = new List<EventAttendee>();
}