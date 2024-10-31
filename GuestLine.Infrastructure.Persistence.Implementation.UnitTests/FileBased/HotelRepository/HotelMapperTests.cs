using FluentAssertions;
using Guestline.Domain;
using Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

namespace GuestLine.Infrastructure.Persistence.Implementation.UnitTests.FileBased.HotelRepository;

public class HotelMapperTests
{
    [TestCase]
    public void MapHotelEntity_ShouldMapCorrectly()
    {
        var dto = new HotelDto{
            Id = "id", 
            Name = "name", 
            RoomTypes = [new RoomTypeDto
            {
                Code = "code1",
                Description = "description1",
                Amenities = ["a1", "a2"],
                Features = [ "f1", "f2"]
            },
            new RoomTypeDto
            {
                Code = "code2",
                Description = "description2",
                Amenities = ["a21", "a22"],
                Features = [ "f21", "f22"]
            }],
            Rooms = [new RoomDto
            {
                RoomId = "r1",
                RoomType = "rt1"
            },
            new RoomDto
            {
                RoomId = "r2",
                RoomType = "rt2"
            },]
        };

        var entity = new HotelMapper().Map(dto);

        entity.Should().BeEquivalentTo(new HotelEntity("id", "name",
            [new RoomType("code1", "description1", ["a1", "a2"], ["f1", "f2"]),
                new RoomType("code2", "description2", ["a21", "a22"], ["f21", "f22"])],
            [new Room("rt1", "r1"), new Room("rt2", "r2")]));
    }

    [TestCase]
    public void MapBookingEntity_ShouldMapCorrectly()
    {
        var dto = new BookingDto
        {
            Arrival = DateTime.Now,
            Departure = DateTime.Now.AddDays(1),
            HotelId = "hotelId",
            RoomRate = "rr",
            RoomType = "rt"
        };

        var entity = new HotelMapper().Map(dto);

        entity.Should().BeEquivalentTo(new BookingEntity("hotelId", dto.Arrival, dto.Departure, "rt", "rr"), opts => opts.Excluding(x => x.Id));
    }
}