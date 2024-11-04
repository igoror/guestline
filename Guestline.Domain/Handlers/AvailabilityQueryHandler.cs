using Guestline.Domain.Handlers.Requests;
using Guestline.Domain.Handlers.Responses;
using Guestline.Infrastructure.Persistence.Contracts;
using Guestline.Infrastructure.Persistence.Contracts.Models;

namespace Guestline.Domain.Handlers;

public class AvailabilityQueryHandler : IQueryHandler<AvailabilityQueryRequest, AvailabilityQueryResponse>
{
    private readonly SearchQueryHandler _searchQueryHandler;

    public AvailabilityQueryHandler(SearchQueryHandler searchQueryHandler)
    {
        _searchQueryHandler = searchQueryHandler;
    }

    public async Task<Result<AvailabilityQueryResponse>> Handle(AvailabilityQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        var result = await _searchQueryHandler.Handle(new SearchQueryRequest(queryRequest.HotelId, queryRequest.StartDate,
            queryRequest.EndDate, queryRequest.RoomType), cancellationToken);
        if (!result.IsSuccess)
            return result.ToFailure<AvailabilityQueryResponse>();
        var minRoomCount = result.Value.AvailableRooms.MinBy(r => r.RoomsCount).RoomsCount;
        return new AvailabilityQueryResponse(minRoomCount);
    }
    
    private static bool CrossingDates(BookingEntity bookingEntity, DateTime start, DateTime end)
    {
        return !(end <= bookingEntity.Arrival || start >= bookingEntity.Departure);
    }
}