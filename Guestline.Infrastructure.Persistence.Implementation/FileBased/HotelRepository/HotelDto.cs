namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class HotelDto
{
    public required string Id {get; set;}
    public required string Name {get; set;}
    public required RoomTypeDto[] RoomTypes {get; set;}
    public required RoomDto[] Rooms {get; set;}
}