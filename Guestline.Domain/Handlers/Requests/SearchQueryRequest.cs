namespace Guestline.Domain.Handlers.Requests;

public record SearchQueryRequest(string HotelId, DateTime StartDate, DateTime EndDate, string RoomType) : QueryRequest;