using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class BookingDto
{
    [JsonPropertyName("hotelId")]
    public required string HotelId {get; set;}
    [JsonPropertyName("arrival")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public required DateTime Arrival {get; set;}
    [JsonPropertyName("departure")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public required DateTime Departure {get; set;}
    [JsonPropertyName("roomType")]
    public required string RoomType {get; set;}
    [JsonPropertyName("roomRate")]
    public required string RoomRate {get; set;}
}