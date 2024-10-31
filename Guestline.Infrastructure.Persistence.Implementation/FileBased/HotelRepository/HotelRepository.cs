using Guestline.Domain;
using Microsoft.Extensions.Options;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased.HotelRepository;

public class HotelRepository(IOptions<FileRepositoryOptions> opts, IMapper<HotelDto, HotelEntity> mapper) 
    : FileRepository<HotelEntity, HotelDto>(opts, mapper);