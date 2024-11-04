using Guestline.Infrastructure.Persistence.Contracts;
using Guestline.Infrastructure.Persistence.Contracts.Models;
using NSubstitute;

namespace Guestline.Domain.UnitTests.Handlers;

public abstract class QueryHandlerTestsBase
{
    
    protected IRepository<BookingEntity> BookingRepo;
    protected IRepository<HotelEntity> HotelRepo;
    
    [SetUp]
    public void SetupBase()
    {
        BookingRepo = Substitute.For<IRepository<BookingEntity>>();
        HotelRepo = Substitute.For<IRepository<HotelEntity>>();
    }
        
    protected string MockHotel(params (string type, int number)[] hotelRooms)
    {
        var id = Guid.NewGuid().ToString();
        var rooms = hotelRooms.Select(param =>
                Enumerable.Range(0, param.number).Select(i => new Room(param.type, i.ToString())).ToList())
            .SelectMany(x => x).ToList();
        var hotelEntity = new HotelEntity(id, id, [], rooms.ToArray());

        var result = Result<IList<HotelEntity>>.Success(new List<HotelEntity> { hotelEntity });
        HotelRepo.FindAsync(Arg.Any<Predicate<HotelEntity>>()).Returns(result);
        return hotelEntity.Id;
    }

    protected void MockReservations(string hotelId, params (DateTime start, DateTime end, string roomType)[] reservations)
    {
        var entities = reservations.Select(r => new BookingEntity(hotelId, r.start, r.end, r.roomType, "rate")).ToList();
        BookingRepo.FindAsync(Arg.Any<Predicate<BookingEntity>>())
            .Returns(args => entities.Where(x => ((Predicate<BookingEntity>)args[0])(x)).ToList());
    }
}