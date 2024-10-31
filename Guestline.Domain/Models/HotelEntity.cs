namespace Guestline.Domain.Models;

public record HotelEntity(string Id, string Name, RoomType[] RoomTypes, Room[] Rooms) : Entity(Id);