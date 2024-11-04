using FluentAssertions;
using Guestline.Domain.Handlers.Requests;

namespace Guestline.Presentation.UnitTests;

public class CommandlineQueryParserTests
{
    [TestCaseSource(nameof(TestCases))]
    public void Parse_ShouldReturnCorrectResults(string command, QueryRequest? expectedResult)
    {
        var parser = new CommandlineQueryParser();

        var result = parser.Parse(command);
        
        switch (expectedResult)
        {
            case AvailabilityQueryRequest ar:
                ar.Should().BeEquivalentTo((AvailabilityQueryRequest) result);
                break;
            case SearchQueryRequest sr:
                sr.Should().BeEquivalentTo((SearchQueryRequest) result);
                break;
            default:
                result.Should().BeNull();
                break;
                
        }
    }
    
    public static object[] TestCases =
    {
        new object?[] { "", null },
        new object?[] { "   ", null },
        new object?[] { "Availability(H1, 20240203, SGL)", new AvailabilityQueryRequest("H1", new DateTime(2024, 02, 03, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 02, 04, 0, 0, 0, DateTimeKind.Utc), "SGL") },
        new object?[] { "Availability(H1, 20240, SGL)", null, },
        new object?[] { "Availability(H1, 20240203-20240304, SGL)", new AvailabilityQueryRequest("H1", new DateTime(2024, 02, 03, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 03, 04, 0, 0, 0, DateTimeKind.Utc), "SGL") },
        new object?[] { "Availability(H1, 20240203-20240304)", null },
        new object?[] { "Availability(H1, 20240203-20240304, )", null },
        new object?[] { "Search(H1, 10, SGL)", new SearchQueryRequest("H1", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(10), "SGL") },
        new object?[] { "Search(H1, 20240203-20240304, SGL)", null }
    };
}