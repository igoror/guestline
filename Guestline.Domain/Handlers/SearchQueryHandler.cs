using Guestline.Domain.Handlers.Requests;
using Guestline.Domain.Handlers.Responses;
using Guestline.Infrastructure.Persistence.Contracts;
using Guestline.Infrastructure.Persistence.Contracts.Models;

namespace Guestline.Domain.Handlers;

public class SearchQueryHandler: IQueryHandler<SearchQueryRequest, SearchQueryResponse>
{
    private readonly IRepository<BookingEntity> _bookingRepository;
    private readonly IRepository<HotelEntity> _hotelRepository;


    public SearchQueryHandler(IRepository<BookingEntity> bookingRepository, IRepository<HotelEntity> hotelRepository)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<SearchQueryResponse>> Handle(SearchQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        if (queryRequest.StartDate.Date == queryRequest.EndDate.Date)
            return new SearchQueryResponse(Enumerable.Empty<(DateTime Start, DateTime End, int RoomsCount)>().ToList());
        
        var hotelResult = await _hotelRepository.FindAsync(h => h.Id == queryRequest.HotelId);
        if (!hotelResult.IsSuccess)
            return hotelResult.ToFailure<SearchQueryResponse>();
        
        if (hotelResult.Value.Count == 0)
            return new SearchQueryResponse(new List<(DateTime Start, DateTime End, int RoomsCount)>());

        var crossingBookingsResult = await _bookingRepository.FindAsync(b =>
            b.HotelId == queryRequest.HotelId &&
            b.RoomType == queryRequest.RoomType &&
            CrossingDates(b, queryRequest.StartDate, queryRequest.EndDate));
        if (!crossingBookingsResult.IsSuccess)
            return crossingBookingsResult.ToFailure<SearchQueryResponse>();

        var allRoomsCount = hotelResult.Value.First().Rooms.Count(r => r.RoomType == queryRequest.RoomType);
        var crossingBookings = crossingBookingsResult.Value
            .SelectMany(b => new List<(string, DateTime)>{("A", b.Arrival), ("D", b.Departure)})
            .GroupBy(x => x.Item2);
        // Arriving/Departuring ordered by event occurence
        var events = new PriorityQueue<(string Type, int Count), (DateTime, int)>();
        
        // Put bookings groupped by date into the queue
        foreach (var crossingBooking in crossingBookings)
        {
            var arrivalCount = crossingBooking.Count(x => x.Item1 == "A");
            var departureCount = crossingBooking.Count(x => x.Item1 == "D");
            // adding pair for the priority, so that if we have more events happening at the very same date we first 
            // handle departures
            events.Enqueue(("A", arrivalCount), (crossingBooking.Key, 2));
            events.Enqueue(("D", departureCount), (crossingBooking.Key, 0));
        }
        events.Enqueue(("Aa", 1), (queryRequest.StartDate, 1));
        events.Enqueue(("Dd", 1), (queryRequest.EndDate, -1));
        
        var result = new List<(DateTime Start, DateTime End, int RoomsCount)>();
        var availableRoomsCount = allRoomsCount;
        var minAvailableRoomsCount = allRoomsCount;
        DateTime? currentPeriodStartDate = null; 
        var started = false;
        while (events.Count != 0)
        {
            events.TryDequeue(out var ev, out var priority);
            var type = ev.Type;
            var count = ev.Count;
            
            if (type == "A")
            {
                TryAddPeriod(started, ref currentPeriodStartDate, priority.Item1, availableRoomsCount, result);
                availableRoomsCount -= count;
                minAvailableRoomsCount = Math.Min(minAvailableRoomsCount, availableRoomsCount);
            }
            else if (type == "D")
            {
                TryAddPeriod(started, ref currentPeriodStartDate, priority.Item1, availableRoomsCount, result);
                availableRoomsCount += count;
            }
            else if (type == "Aa")
            {
                started = true;
                currentPeriodStartDate = priority.Item1;
            }
            else if (type == "Dd")
            {
                TryAddPeriod(started, ref currentPeriodStartDate, priority.Item1, availableRoomsCount, result);
                break;
            }
        }

        return new SearchQueryResponse(result);
    }

    private static void TryAddPeriod(bool started, ref DateTime? start, DateTime end, int roomsCount, List<(DateTime Start, DateTime End, int RoomsCount)> acc)
    {
        if (!started || start == end)
            return;
        acc.Add((start!.Value, end, roomsCount));
        start = end;
    }
    
    private static bool CrossingDates(BookingEntity bookingEntity, DateTime start, DateTime end)
    {
        return !(end <= bookingEntity.Arrival || start >= bookingEntity.Departure);
    }
}