using Customer.Api.Controllers;
using Customer.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Customer.Api.Tests.Controllers;

public class SystemControllerTests : IDisposable
{
    private readonly Mock<IWebHostEnvironment> _envMock = new();
    private readonly Mock<ILogger<SystemController>> _loggerMock = new();
    private readonly SystemController _controller;
    private readonly string _tempVersionFilePath;
    private readonly string _tempRoot;

    public SystemControllerTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempRoot);
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
        _envMock.Setup(e => e.WebRootPath).Returns((string)null!);
        _envMock.Setup(e => e.EnvironmentName).Returns("Development");

        _controller = new SystemController(_envMock.Object, _loggerMock.Object);
        _tempVersionFilePath = Path.Combine(_tempRoot, "version.txt");
    }

    public void Dispose()
    {
        if (File.Exists(_tempVersionFilePath))
        {
            File.Delete(_tempVersionFilePath);
        }

        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, true);
        }

        _controller.Dispose();
    }

    [Fact]
    public void Index_ShouldReturnPong()
    {
        var result = _controller.Index() as OkObjectResult;

        result.Should().NotBeNull();
        result!.Value.Should().Be("pong");
    }

    [Fact]
    public void GetVersion_ShouldReturnUnknown_WhenFileDoesNotExist()
    {
        var result = _controller.GetVersion() as OkObjectResult;

        result.Should().NotBeNull();
        var model = result!.Value as VersionModel;
        model.Should().NotBeNull();
        model!.BuildVersion.Should().Be("unknown");
        model.Environment.Should().Be("Development");
    }

    [Fact]
    public void GetVersion_ShouldReturnFileContent_WhenFileExists()
    {
        File.WriteAllText(_tempVersionFilePath, "1.2.3\n\n");

        var result = _controller.GetVersion() as OkObjectResult;

        result.Should().NotBeNull();
        var model = result!.Value as VersionModel;
        model.Should().NotBeNull();
        model!.BuildVersion.Should().Be("1.2.3");
        model.Environment.Should().Be("Development");
    }

    [Fact]
    public void GetVersion_ShouldReadAwsEnvVars()
    {
        Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");
        Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-west-2");
        Environment.SetEnvironmentVariable("AWS_EXECUTION_ENV", "AWS_Lambda_dotnet");

        try
        {
            var result = _controller.GetVersion() as OkObjectResult;

            result.Should().NotBeNull();
            var model = result!.Value as VersionModel;
            model!.AwsRegion.Should().Be("us-east-1");
            model.DefaultAwsRegion.Should().Be("us-west-2");
            model.AwsExecutionEnvironment.Should().Be("AWS_Lambda_dotnet");
        }
        finally
        {
            Environment.SetEnvironmentVariable("AWS_REGION", null);
            Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", null);
            Environment.SetEnvironmentVariable("AWS_EXECUTION_ENV", null);
        }
    }
}
