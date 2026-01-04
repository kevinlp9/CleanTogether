namespace CleanTogether.API.Entities;

public class EventAttendee
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public CleaningEvent CleaningEvent { get; set; } = null!;
}