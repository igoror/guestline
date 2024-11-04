using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class RoomTypeDto
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("amenities")]
    public required string[] Amenities { get; set; }
    [JsonPropertyName("features")]
    public required string[] Features { get; set; }
}
