using Guestline.Infrastructure.Persistence.Contracts.Models;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class HotelMapper : IMapper<HotelDto, HotelEntity>, IMapper<BookingDto, BookingEntity>
{
    public HotelEntity Map(HotelDto from)
    {
        return new HotelEntity(from.Id, from.Name,
            from.RoomTypes.Select(rt =>
                new RoomType(rt.Code, rt.Description, rt.Amenities.ToArray(), rt.Features.ToArray())).ToArray(),
            from.Rooms.Select(r => new Room(r.RoomType, r.RoomId)).ToArray());
    }

    public BookingEntity Map(BookingDto from)
    {
        return new BookingEntity(from.HotelId, from.Arrival, from.Departure, from.RoomType, from.RoomRate);
    }
}