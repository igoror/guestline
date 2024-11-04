using FluentAssertions;
using Guestline.Domain.Handlers;
using Guestline.Domain.Handlers.Requests;

namespace Guestline.Domain.UnitTests.Handlers;

public class SearchQueryHandlerTests : QueryHandlerTestsBase
{
    private SearchQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _handler = new SearchQueryHandler(BookingRepo, HotelRepo);
    }
    
    [Test]
    public async Task Handle_No_Reservations_Returns_FullPeriod()
    {
        var n = DateTime.UtcNow;
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId);

        var result = await _handler.Handle(new SearchQueryRequest(hotelId, n, n.AddDays(5), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableRooms.Should().BeEquivalentTo(new List<(DateTime Start, DateTime End, int RoomsCount)>
        {
            (n, n.AddDays(5), 10)
        });
    }
    
    [Test]
    public async Task Handle_ComplexScenario_ReturnsCorrectResult()
    {
        var n = DateTime.UtcNow;
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId, 
            (n, n.AddDays(2), "Type"),
            (n, n.AddDays(2), "Type"),
            (n, n.AddDays(2), "Type"),
            (n, n.AddDays(2), "Type"),
            (n, n.AddDays(6), "Type"),
            (n.AddDays(1), n.AddDays(2), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type")
            
            );

        var result = await _handler.Handle(new SearchQueryRequest(hotelId, n.AddDays(-5), n.AddDays(7), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableRooms.Should().BeEquivalentTo(new List<(DateTime Start, DateTime End, int RoomsCount)>
        {
            (n.AddDays(-5), n, 10),
            (n, n.AddDays(1), 5),
            (n.AddDays(1), n.AddDays(2), 4),
            (n.AddDays(2), n.AddDays(3), 9),
            (n.AddDays(3), n.AddDays(4), -1),
            (n.AddDays(4), n.AddDays(5), 4),
            (n.AddDays(5), n.AddDays(6), 9),
            (n.AddDays(6), n.AddDays(7), 10)
        });
    }
}