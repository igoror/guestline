## Prerequisits:
- net8.0
- C# 12
- NuGet

## How to run:
- `dotnet test` to run all the tests
- There are sample data files prepared under `Guestline.Presentation/SampleData` directory. In order to run type ` dotnet run --project Guestline.Presentation --hotels Guestline.Presentation/SampleData/hotels_data2.json --bookings Guestline.Presentation/SampleData/bookings_data2.json`

```
Representation of bookings_data2.json and hotels_data2.json

There is H1 hotel that contains 10 SGL and 2 DBL rooms (not booked at all). SGL rooms are booked as follows:
# - single day
[ - start of booking
) - end of booking

First day is 2024-11-04

###############
[ )
[ )
[ )
[ )
[      )
 [)
    [)
    [)
    [)
    [)
    [)
    [ )
    [ )
    [ )
    [ )
    [ )
```

## Notes:
It wasn't clear from the task description whether we should return overbooked periods for `Search` command . To be consistent with `Availability` command I decided to return overbooked periods as well. It's a matter of adding one simple `if` statement in the code to get rid of them.

Code responsible for handling queries lives in `Guestline.Domain.AvailabilityQueryHandler` and `Guestline.Domain.SearchQueryHandler`. Rest is a glue code.
