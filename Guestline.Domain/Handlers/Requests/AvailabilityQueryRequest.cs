namespace Guestline.Domain.Handlers.Requests;

public record AvailabilityQueryRequest(string HotelId, DateTime StartDate, DateTime EndDate, string RoomType) : QueryRequest;