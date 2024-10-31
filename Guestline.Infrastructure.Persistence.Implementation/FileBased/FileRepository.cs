using System.Text.Json;
using System.Text.Json.Serialization;
using Guestline.Domain;
using Guestline.Infrastructure.Persistence.Contracts;
using Microsoft.Extensions.Options;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased;

public class FileRepository<T, TDto>(IOptions<FileRepositoryOptions> opts, IMapper<TDto, T> mapper) : IRepository<T>
    where T : Entity
{
    private readonly FileRepositoryOptions _opts = opts.Value;

    public async Task<Result<IList<T>>> FindAsync(Predicate<T> predicate)
    {
        try
        {
            using var streamReader = new StreamReader(_opts.FileLocation);
            var deserialized = await JsonSerializer.DeserializeAsync<List<TDto>>(streamReader.BaseStream, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.Never });
            if (deserialized == null)
                return Result<IList<T>>.Failure($"Could not deserialize the file content: {_opts.FileLocation}");
            return deserialized.Select(mapper.Map).Where(e => predicate(e)).ToList();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}