namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class BookingDto
{
    public required string HotelId {get; set;}
    public required DateTime Arrival {get; set;}
    public required DateTime Departure {get; set;}
    public required string RoomType {get; set;}
    public required string RoomRate {get; set;}
}