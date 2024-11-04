using FluentAssertions;
using Guestline.Domain.Handlers;
using Guestline.Domain.Handlers.Requests;

namespace Guestline.Domain.UnitTests.Handlers;

public class AvailabilityQueryHandlerTests : QueryHandlerTestsBase
{
    private AvailabilityQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        var searchQueryHandler = new SearchQueryHandler(BookingRepo, HotelRepo);
        _handler = new AvailabilityQueryHandler(searchQueryHandler);
    }

    [Test]
    public async Task Handle_No_Reservations_Returns_Full()
    {
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId);

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(10);
    }
    
    [Test]
    public async Task Handle_MaxOneRoomBookedAtTheTime_ReturnsAllMinus1()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId, 
            (n, n.AddDays(1), "Type"),
            (n.AddDays(1), n.AddDays(2), "Type"),
            (n.AddDays(2), n.AddDays(3), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n, n.AddDays(100), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(9);
    }
    
    [Test]
    public async Task Handle_OnlyOneRoom_FullyBooked_Returns0()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 1));
        MockReservations(hotelId, 
            (n, n.AddDays(1), "Type"),
            (n.AddDays(1), n.AddDays(2), "Type"),
            (n.AddDays(2), n.AddDays(3), "Type"),
            (n.AddDays(3), n.AddDays(4), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n, n.AddDays(100), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(0);
    }
    
    [Test]
    public async Task Handle_RoomsBookedBeforeRequestedDay_ReturnsCorrectResult()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId,
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(100), "Type"),
            (n.AddDays(3), n.AddDays(100), "Type"),
            (n.AddDays(6), n.AddDays(100), "Type"),
            (n.AddDays(9), n.AddDays(12), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n.AddDays(6), n.AddDays(100), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(6);
    }
    
    [Test]
    public async Task Handle_RoomsBookedAfterRequestedDay_ReturnsCorrectResult()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId,
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(3), "Type"),
            (n, n.AddDays(100), "Type"),
            (n.AddDays(3), n.AddDays(100), "Type"),
            (n.AddDays(6), n.AddDays(100), "Type"),
            (n.AddDays(9), n.AddDays(12), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"),
            (n.AddDays(100), n.AddDays(103), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n.AddDays(6), n.AddDays(100), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(6);
    }
    
    [Test]
    public async Task Handle_Overbooked_ReturnsNegative()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 1));
        MockReservations(hotelId, 
            (n, n.AddDays(1), "Type"),
            (n, n.AddDays(1), "Type"),
            (n, n.AddDays(1), "Type"),
            (n, n.AddDays(1), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n, n.AddDays(100), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(-3);
    }
    
    [Test]
    public async Task Handle_NoCrossingReservation_ReturnsFull()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 100));
        MockReservations(hotelId, 
            (n, n.AddDays(1), "Type"),
            (n, n.AddDays(1), "Type"),
            (n.AddDays(3), n.AddDays(5), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n.AddDays(1), n.AddDays(3), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(100);
    }
    
    [Test]
    public async Task Handle_RangeCheck_ShouldReturnCorrectResult()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 100));
        MockReservations(hotelId, 
            (n, n.AddDays(5), "Type"),
            (n, n.AddDays(3), "Type"),
            (n.AddDays(4), n.AddDays(5), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n.AddDays(3), n.AddDays(4), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(99);
    }
    
    [Test]
    public async Task Handle_FirstHandleDepartures_ShouldReturnCorrectResult()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 3));
        MockReservations(hotelId, 
            (n.AddDays(5), n.AddDays(6), "Type"),
            (n, n.AddDays(5), "Type"),
            (n, n.AddDays(5), "Type"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n, n.AddDays(6), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(1);
    }
    
    [Test]
    public async Task Handle_RoomTypesAreDoNotInterfereWithEachOther_ShouldReturnCorrectResult()
    {
        var n = DateTime.UtcNow.Date;
        var hotelId = MockHotel(("Type", 3));
        MockReservations(hotelId, 
            (n.AddDays(5), n.AddDays(6), "Type"),
            (n, n.AddDays(5), "Type"),
            (n, n.AddDays(5), "Type"),
            (n, n.AddDays(5), "Type2"),
            (n, n.AddDays(5), "Type2"),
            (n, n.AddDays(5), "Type2"));

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, n, n.AddDays(6), "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(1);
    }
}