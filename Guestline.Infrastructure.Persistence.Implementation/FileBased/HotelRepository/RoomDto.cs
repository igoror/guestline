using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class RoomDto
{
    [JsonPropertyName("roomType")]
    public required string RoomType { get; set; }
    [JsonPropertyName("roomId")]
    public required string RoomId { get; set; }
}