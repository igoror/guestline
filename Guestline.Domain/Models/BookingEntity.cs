namespace Guestline.Domain.Models;

public record BookingEntity(string HotelId, DateTime Arrival, DateTime Departure, string RoomType, string RoomRate) : Entity(Guid.NewGuid().ToString());