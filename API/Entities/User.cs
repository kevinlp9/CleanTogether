namespace CleanTogether.API.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public ICollection<EventAttendee> AttendingEvents { get; set; }
}