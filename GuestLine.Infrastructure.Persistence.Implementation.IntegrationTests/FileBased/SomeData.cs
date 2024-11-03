using Guestline.Infrastructure.Persistence.Contracts.Models;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public record NestedObject(string Property);

public record SomeData(string Id, string SomeProperty, NestedObject NestedObject) : Entity(Id);