namespace Guestline.Infrastructure.Persistence.Contracts.Models;

public record RoomType(string Code, string Description, string[] Amenities, string[] Features);