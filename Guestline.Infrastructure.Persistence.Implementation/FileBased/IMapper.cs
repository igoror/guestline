namespace Guestline.Infrastructure.Persistence.Implementation.FileBased;

public interface IMapper<TFrom, TTo>
{
    public TTo Map(TFrom from);
}