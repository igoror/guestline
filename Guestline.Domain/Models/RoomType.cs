namespace Guestline.Domain.Models;

public record RoomType(string Code, string Description, string[] Amenities, string[] Features);