using Guestline.Infrastructure.Persistence.Implementation.FileBased;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public class SomeDataMapper : IMapper<SomeDataDto, SomeData>
{
    public SomeData Map(SomeDataDto from)
    {
        return new SomeData(from.Id, from.SomeProperty, new NestedObject(from.NestedObject.NestedObjectProperty));
    }
}