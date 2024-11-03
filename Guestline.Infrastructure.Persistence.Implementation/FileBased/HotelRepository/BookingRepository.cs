using Guestline.Infrastructure.Persistence.Contracts.Models;
using Microsoft.Extensions.Options;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class BookingRepository(IOptions<FileRepositoryOptions> opts, IMapper<BookingDto, BookingEntity> mapper) 
    : FileRepository<BookingEntity, BookingDto>(opts, mapper);