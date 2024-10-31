using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class RoomTypeDto
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public required string[] Amenities { get; set; }
    public required string[] Features { get; set; }
}
