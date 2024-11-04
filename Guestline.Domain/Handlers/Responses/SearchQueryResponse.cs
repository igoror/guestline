namespace Guestline.Domain.Handlers.Responses;

public record SearchQueryResponse(List<(DateTime Start, DateTime End, int RoomsCount)> AvailableRooms);