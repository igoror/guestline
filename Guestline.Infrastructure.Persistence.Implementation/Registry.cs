using Microsoft.Extensions.DependencyInjection;

namespace Guestline.Infrastructure.Persistence.Implementation;

public class Registry
{
    public IServiceCollection RegisterPersistence(IServiceCollection serviceCollection)
    {
        //serviceCollection.AddSingleton<>();
        return serviceCollection;
    }
}