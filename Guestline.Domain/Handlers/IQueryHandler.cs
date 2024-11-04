using Guestline.Infrastructure.Persistence.Contracts;

namespace Guestline.Domain.Handlers;

public interface IQueryHandler<TRequest, TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}