using FluentAssertions;
using Guestline.Domain;
using Guestline.Domain.Models;
using Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public class HotelRepositoryTests : RepositoryTestsBase<HotelRepository>
{
    protected override HotelRepository CreateRepository(string filePath)
    {
        return new HotelRepository(BuildOptions(ValidDataPath("hotels_data.json")), new HotelMapper());
    }

    [TestCase]
    public async Task FindAsync_ShouldReturnCorrectBookings()
    {
        var result = await FileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo([
            new HotelEntity("H1", "Hotel California",
                new[]
                {
                    new RoomType("SGL", "Single Room", new[] { "WiFi", "TV" }, new[] { "Non-smoking" }),
                    new RoomType("DBL", "Double Room", new[] { "WiFi", "TV", "Minibar" },
                        new[] { "Non-smoking", "Sea View" })
                },
                new[]
                {
                    new Room("SGL", "101"),
                    new Room("SGL", "102"),
                    new Room("DBL", "201"),
                    new Room("DBL", "202")
                })
        ]);
    }
}