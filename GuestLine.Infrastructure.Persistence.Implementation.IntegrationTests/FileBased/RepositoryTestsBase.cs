using Guestline.Infrastructure.Persistence.Implementation.FileBased;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

public abstract class RepositoryTestsBase<T>
{
    protected T FileRepository;
    
    [SetUp]
    protected void Setup()
    {
        FileRepository = CreateRepository(ValidDataPath("some_data.json"));
    }
    
    protected string ValidDataPath(string fileName)
    {
        return $"{Path.Join(Environment.CurrentDirectory)}/Data/{fileName}";
    }

    protected IOptions<FileRepositoryOptions> BuildOptions(string filePath)
    {
        var options = new FileRepositoryOptions { FileLocation = filePath };
        var iOptions = Substitute.For<IOptions<FileRepositoryOptions>>();
        iOptions.Value.Returns(options);
        return iOptions;
    }

    protected abstract T CreateRepository(string filePath);
}