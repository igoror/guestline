using Guestline.Domain.Handlers.Requests;
using Guestline.Domain.Handlers.Responses;
using Guestline.Infrastructure.Persistence.Contracts;
using Guestline.Infrastructure.Persistence.Contracts.Models;

namespace Guestline.Domain.Handlers;

public class AvailabilityQueryHandler : IQueryHandler<AvailabilityQueryRequest, AvailabilityQueryResponse>
{
    private readonly IRepository<BookingEntity> _bookingRepository;
    private readonly IRepository<HotelEntity> _hotelRepository;

    public AvailabilityQueryHandler(IRepository<BookingEntity> bookingRepository, IRepository<HotelEntity> hotelRepository)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<AvailabilityQueryResponse>> Handle(AvailabilityQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        var hotelResult = await _hotelRepository.FindAsync(h => h.Id == queryRequest.HotelId);
        if (!hotelResult.IsSuccess)
            return hotelResult.ToFailure<AvailabilityQueryResponse>();
        
        if (hotelResult.Value.Count == 0)
            return new AvailabilityQueryResponse(0);

        var crossingBookingsResult = await _bookingRepository.FindAsync(b =>
            b.HotelId == queryRequest.HotelId &&
            b.RoomType == queryRequest.RoomType &&
            CrossingDates(b, queryRequest.StartDate, queryRequest.EndDate));
        if (!crossingBookingsResult.IsSuccess)
            return crossingBookingsResult.ToFailure<AvailabilityQueryResponse>();

        var allRoomsCount = hotelResult.Value.First().Rooms.Count(r => r.RoomType == queryRequest.RoomType);
        var crossingBookings = crossingBookingsResult.Value;
        // Arriving/Departuring ordered by event occurence
        var events = new PriorityQueue<string, (DateTime, int)>();
        
        // Put bookings into the queue
        foreach (var crossingBooking in crossingBookings)
        {
            // adding pair for the priority, so that if we have more events happening at the very same date we first 
            // handle departures
            events.Enqueue("A", (crossingBooking.Arrival, 1));
            events.Enqueue("D", (crossingBooking.Departure, 0));
        }

        var availableRoomsCount = allRoomsCount;
        var minAvailableRoomsCount = allRoomsCount;
        while (events.Count != 0)
        {
            var ev = events.Dequeue();
            if (ev == "A")
            {
                --availableRoomsCount;
                minAvailableRoomsCount = Math.Min(minAvailableRoomsCount, availableRoomsCount);
            }
            else if (ev == "D")
            {
                ++availableRoomsCount;
            }
        }

        return new AvailabilityQueryResponse(minAvailableRoomsCount);
    }
    
    private static bool CrossingDates(BookingEntity bookingEntity, DateTime start, DateTime end)
    {
        return !(end <= bookingEntity.Arrival || start >= bookingEntity.Departure);
    }
}