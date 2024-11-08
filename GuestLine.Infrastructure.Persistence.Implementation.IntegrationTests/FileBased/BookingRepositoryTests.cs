using FluentAssertions;
using Guestline.Infrastructure.Persistence.Contracts.Models;
using Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public class BookingRepositoryTests : RepositoryTestsBase<BookingRepository>
{
    protected override BookingRepository CreateRepository(string filePath)
    {
        return new BookingRepository(BuildOptions(ValidDataPath("bookings_data.json")), new HotelMapper());
    }

    [TestCase]
    public async Task FindAsync_ShouldReturnCorrectBookings()
    {
        var result = await FileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo([
            new BookingEntity("H1", new DateTime(2024, 9, 1),
                new DateTime(2024, 9, 3, 0,0,0, DateTimeKind.Utc), "DBL", "Prepaid"),
            new BookingEntity("H1", new DateTime(2024, 9, 2, 0, 0,0, DateTimeKind.Utc),
                new DateTime(2024, 9, 5), "SGL", "Standard")
        ], opts => opts.Excluding(e => e.Id));
    }
}