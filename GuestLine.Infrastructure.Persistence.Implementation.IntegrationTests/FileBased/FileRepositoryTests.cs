using System.Text.Json;
using FluentAssertions;
using Guestline.Infrastructure.Persistence.Implementation.FileBased;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public class FileRepositoryTests
{
    private SomeDataFileRepository _fileRepository;

    private class SomeDataFileRepository(IOptions<FileRepositoryOptions> opts) :
        FileRepository<SomeData, SomeDataDto>(opts, new SomeDataMapper());
    
    [SetUp]
    public void Setup()
    {
        _fileRepository = CreateFileRepository(ValidDataPath("some_data.json"));
    }

    [TestCase]
    public async Task FindAsync_WhenPredicateAlwaysPasses_ReturnsAllTheData()
    {
        var result = await _fileRepository.FindAsync(_ => true);

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
        var result = await _fileRepository.FindAsync(_ => false);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [TestCase]
    public async Task FindAsync_WhenIncorrectPath_ReturnsFailure()
    {
        var fileRepository = CreateFileRepository("some-path");
        
        var result = await fileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().NotBeNull();
    }
    
    [TestCase]
    public async Task FindAsync_WhenIncompatibleValue_ReturnsFailure()
    {
        var fileRepository = CreateFileRepository(ValidDataPath("some_other_data.json"));
        
        var result = await fileRepository.FindAsync(_ => true);

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<JsonException>();
    }

    private static SomeDataFileRepository CreateFileRepository(string dataFilePath)
    {
        var options = new FileRepositoryOptions { FileLocation = dataFilePath };
        var iOptions = Substitute.For<IOptions<FileRepositoryOptions>>();
        iOptions.Value.Returns(options);
        return new SomeDataFileRepository(iOptions);
    }

    private static string ValidDataPath(string fileName)
    {
        return $"{Path.Join(Environment.CurrentDirectory)}/Data/{fileName}";
    }
}