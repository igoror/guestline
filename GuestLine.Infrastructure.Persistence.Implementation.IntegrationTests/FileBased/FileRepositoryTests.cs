using System.Text.Json;
using FluentAssertions;
using Guestline.Infrastructure.Persistence.Implementation.FileBased;
using Microsoft.Extensions.Options;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public class FileRepositoryTests : RepositoryTestsBase<FileRepositoryTests.SomeDataFileRepository>
{
    public class SomeDataFileRepository(IOptions<FileRepositoryOptions> opts) :
        FileRepository<SomeData, SomeDataDto>(opts, new SomeDataMapper());

    [TestCase]
    public async Task FindAsync_WhenPredicateAlwaysPasses_ReturnsAllTheData()
    {
        var result = await FileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(
            new List<SomeData>
            {
                new SomeData("id1", "property_val", new NestedObject("nested_value")),
                new SomeData("id2", "property_val2", new NestedObject("nested_value2"))
            });
    }
    
    [TestCase]
    public async Task FindAsync_WhenPredicateAlwaysRejects_ReturnsEmptyData()
    {
        var result = await FileRepository.FindAsync(_ => false);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [TestCase]
    public async Task FindAsync_WhenIncorrectPath_ReturnsFailure()
    {
        var fileRepository = CreateRepository("some-path");
        
        var result = await fileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
    }
    
    [TestCase]
    public async Task FindAsync_WhenIncompatibleValue_ReturnsFailure()
    {
        var fileRepository = CreateRepository(ValidDataPath("some_other_data.json"));
        
        var result = await fileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<JsonException>();
    }

    protected override SomeDataFileRepository CreateRepository(string filePath)
    {
        return new SomeDataFileRepository(BuildOptions(filePath));
    }
}