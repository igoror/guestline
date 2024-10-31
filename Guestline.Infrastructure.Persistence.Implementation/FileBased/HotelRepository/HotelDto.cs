using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class HotelDto
{
    [JsonPropertyName("id")]
    public required string Id {get; set;}
    [JsonPropertyName("name")]
    public required string Name {get; set;}
    [JsonPropertyName("roomTypes")]
    public required RoomTypeDto[] RoomTypes {get; set;}
    [JsonPropertyName("rooms")]
    public required RoomDto[] Rooms {get; set;}
}