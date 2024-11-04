using System.Globalization;
using System.Text.RegularExpressions;
using Guestline.Domain.Handlers.Requests;

namespace Guestline.Presentation;

public class CommandlineQueryFactory
{
    private readonly Regex AvailabilityRegex = new("^Availability\\((.+), (.+), (.+)\\)$", RegexOptions.Compiled);
    private readonly Regex SearchRegex = new("^Availability\\((.+), ([1-9]\\d{7})-?([1-9]\\d{7})?, (.+)\\)$", RegexOptions.Compiled);
    private readonly Regex DateRangeRegex = new("([1-9]\\d{7})-?([1-9]\\d{7})?", RegexOptions.Compiled);

    
    public QueryRequest? GetNext(string command)
    {
        if (AvailabilityRegex.IsMatch(command))
        {
            var match = AvailabilityRegex.Matches(command).First();
            var hotel = match.Groups[1].Value;
            var dateRange = ParseDateRange(match.Groups[2].Value);
            if (dateRange == null)
                return null;
            
            var roomType = match.Groups[3].Value;
            return new AvailabilityQueryRequest(hotel, dateRange.Value.Start, dateRange.Value.End, roomType); 
        }
        if (SearchRegex.IsMatch(command))
        {
            var match = SearchRegex.Matches(command).First();
            var hotel = match.Groups[1].Value;
            if (!int.TryParse(match.Groups[2].Value, CultureInfo.InvariantCulture, out var days))
                return null;
            
            var roomType = match.Groups[3].Value;
            var now = DateTime.UtcNow.Date;
            return new SearchQueryRequest(hotel, now, now.AddDays(days), roomType); 
        }

        return null;
    }

    private (DateTime Start, DateTime End)? ParseDateRange(string value)
    {
        var match = DateRangeRegex.Matches(value).FirstOrDefault();
        if (match == null)
            return null;
        
        return match.Groups[2].Captures.Count switch
        {
            
            0 => (ParseDateTime(match.Groups[1].Value),
                ParseDateTime(match.Groups[1].Value).AddDays(1)),
            1 => (ParseDateTime(match.Groups[1].Value),
                ParseDateTime(match.Groups[2].Value)),
            _ => null
        };
    }

    private DateTime ParseDateTime(string date)
    {
        return DateTime.SpecifyKind(DateTime.ParseExact(date, "yyyyMMdd", null), DateTimeKind.Utc);
    }
}