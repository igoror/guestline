using FluentAssertions;
using Guestline.Domain.Handlers;
using Guestline.Domain.Handlers.Requests;
using Guestline.Infrastructure.Persistence.Contracts;
using Guestline.Infrastructure.Persistence.Contracts.Models;
using NSubstitute;

namespace Guestline.Domain.UnitTests.Handlers;

public class AvailabilityQueryHandlerTests
{
    private IRepository<BookingEntity> _bookingRepo;
    private IRepository<HotelEntity> _hotelRepo;
    private AvailabilityQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _bookingRepo = Substitute.For<IRepository<BookingEntity>>();
        _hotelRepo = Substitute.For<IRepository<HotelEntity>>();
        _handler = new AvailabilityQueryHandler(_bookingRepo, _hotelRepo);
    }

    [Test]
    public async Task Handle_No_Reservations_Returns_Full()
    {
        var hotelId = MockHotel(("Type", 10));
        MockReservations(hotelId);

        var result = await _handler.Handle(new AvailabilityQueryRequest(hotelId, DateTime.Now, DateTime.Today, "Type"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailabilityCount.Should().Be(10);
    }
    
    [Test]
    public async Task Handle_MaxOneRoomBookedAtTheTime_ReturnsAllMinus1()
    {
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
        var n = DateTime.Now.Date;
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
    
    private string MockHotel(params (string type, int number)[] hotelRooms)
    {
        var id = Guid.NewGuid().ToString();
        var rooms = hotelRooms.Select(param =>
                Enumerable.Range(0, param.number).Select(i => new Room(param.type, i.ToString())).ToList())
            .SelectMany(x => x).ToList();
        var hotelEntity = new HotelEntity(id, id, [], rooms.ToArray());

        var result = Result<IList<HotelEntity>>.Success(new List<HotelEntity> { hotelEntity });
        _hotelRepo.FindAsync(Arg.Any<Predicate<HotelEntity>>()).Returns(result);
        return hotelEntity.Id;
    }

    private void MockReservations(string hotelId, params (DateTime start, DateTime end, string roomType)[] reservations)
    {
        var entities = reservations.Select(r => new BookingEntity(hotelId, r.start, r.end, r.roomType, "rate")).ToList();
        _bookingRepo.FindAsync(Arg.Any<Predicate<BookingEntity>>())
            .Returns(args => entities.Where(x => ((Predicate<BookingEntity>)args[0])(x)).ToList());
    }
}