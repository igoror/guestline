﻿// See https://aka.ms/new-console-template for more information

using Guestline.Domain.Handlers;
using Guestline.Domain.Handlers.Requests;
using Guestline.Infrastructure.Persistence.Implementation.FileBased;
using Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;
using Guestline.Presentation;
using Microsoft.Extensions.Options;

var options = ParseArgs(args);
await Process();

async Task Process()
{
    var commandFactory = new CommandlineQueryParser();
    var bookingRepository = new BookingRepository(Options.Create(new FileRepositoryOptions()
        { FileLocation = options.BookingsPath }), new HotelMapper());
    var hotelRepository = new HotelRepository(Options.Create(new FileRepositoryOptions()
        { FileLocation = options.HotelsPath }), new HotelMapper());
    var searchQueryHandler = new SearchQueryHandler(bookingRepository, hotelRepository);
    var availabilityQueryHandler = new AvailabilityQueryHandler(searchQueryHandler);
    
    while (true)
    {
        var currentInput = Console.ReadLine();
        if (string.IsNullOrEmpty(currentInput))
            return;

        var query = commandFactory.Parse(currentInput);
        if (query == null)
        {
            Console.WriteLine("Invalid input provided");
            continue;
        }

        switch (query)
        {
            case AvailabilityQueryRequest req:
            {
                var response = await availabilityQueryHandler.Handle(req, default);
                if (!response.IsSuccess)
                    Console.WriteLine($"Something wrong happened {response.Exception?.Message ?? response.Error}");
                Console.WriteLine(response.Value.AvailabilityCount);
                break;
            }
            case SearchQueryRequest req:
            {
                var response = await searchQueryHandler.Handle(req, default);
                if (!response.IsSuccess)
                    Console.WriteLine($"Something wrong happened {response.Exception?.Message ?? response.Error}");
                Console.WriteLine(string.Join($",{Environment.NewLine}", response.Value.AvailableRooms.Select(x => $"({x.Start.ToString("yyyyMMdd")}-{x.End.ToString("yyyyMMdd")}, {x.RoomsCount})")));
                break;
            }
        }
        
    }
}

(string HotelsPath, string BookingsPath) ParseArgs(string[] args)
{
    var requiredOptions = new List<string> { "--hotels", "--bookings" };
    if (requiredOptions.Any(x => !args.Contains(x)))
        throw new ApplicationException($"{string.Join(", ", requiredOptions)} options are required.");

    return (ParseOption(args, requiredOptions[0]), ParseOption(args, requiredOptions[1]));
}

string ParseOption(string[] args, string option)
{
    var indexOfHotels = Array.IndexOf(args, option);
    if (indexOfHotels + 1 >= args.Length)
        throw new ApplicationException($"Incorrect argument value {option}");
    return args[indexOfHotels + 1];
}