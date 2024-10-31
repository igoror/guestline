namespace Guestline.Domain;

public record BookingEntity(string HotelId, DateTime Arrival, DateTime Departure, string RoomType, string RoomRate);