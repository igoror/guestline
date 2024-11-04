namespace Guestline.Infrastructure.Persistence.Contracts.Models;

public record HotelEntity(string Id, string Name, RoomType[] RoomTypes, Room[] Rooms) : Entity(Id);