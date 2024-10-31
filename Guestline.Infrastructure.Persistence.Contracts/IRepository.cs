using Guestline.Domain;

namespace Guestline.Infrastructure.Persistence.Contracts;

public interface IRepository<T> where T : Entity
{
    Task<Result<IList<T>>> FindAsync(Predicate<T> predicate);
}