using System;
using System.IO;
using Customer.Api.Controllers;
using Customer.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Customer.Api.Tests.Controllers
{
    public class SystemControllerTests
    {
        private Mock<IWebHostEnvironment> _envMock;
        private Mock<ILogger<SystemController>> _loggerMock;
        private SystemController _controller;
        private string _tempVersionFilePath;

        [SetUp]
        public void SetUp()
        {
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<SystemController>>();

            // Setup temp content root
            string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
            _envMock.Setup(e => e.ContentRootPath).Returns(tempRoot);
            _envMock.Setup(e => e.WebRootPath).Returns((string)null);
            _envMock.Setup(e => e.EnvironmentName).Returns("Development");

            _controller = new SystemController(_envMock.Object, _loggerMock.Object);

            _tempVersionFilePath = Path.Combine(tempRoot, "version.txt");
        }

        [TearDown]
        public void TearDown()
        {
            if (System.IO.File.Exists(_tempVersionFilePath))
                System.IO.File.Delete(_tempVersionFilePath);

            _controller?.Dispose();
        }

        [Test]
        public void Index_ShouldReturnPong()
        {
            // Act
            var result = _controller.Index() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be("pong");
        }

        [Test]
        public void GetVersion_ShouldReturnUnknown_WhenFileDoesNotExist()
        {
            // Act
            var result = _controller.GetVersion() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Value as VersionModel;
            model.Should().NotBeNull();
            model!.BuildVersion.Should().Be("unknown");
            model.Environment.Should().Be("Development");
        }

        [Test]
        public void GetVersion_ShouldReturnFileContent_WhenFileExists()
        {
            // Arrange
            var expectedVersion = "1.2.3";
            File.WriteAllText(_tempVersionFilePath, expectedVersion + "\n\n");

            // Act
            var result = _controller.GetVersion() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Value as VersionModel;
            model.Should().NotBeNull();
            model!.BuildVersion.Should().Be("1.2.3");
            model.Environment.Should().Be("Development");
        }

        [Test]
        public void GetVersion_ShouldReadAwsEnvVars()
        {
            // Arrange
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");
            Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-west-2");
            Environment.SetEnvironmentVariable("AWS_EVECUTION_ENV", "AWS_Lambda_dotnet");

            // Act
            var result = _controller.GetVersion() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Value as VersionModel;
            model!.AwsRegion.Should().Be("us-east-1");
            model.DefaultAwsRegion.Should().Be("us-west-2");
            model.AwsExecutionEnvironment.Should().Be("AWS_Lambda_dotnet");

            // Clean up
            Environment.SetEnvironmentVariable("AWS_REGION", null);
            Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", null);
            Environment.SetEnvironmentVariable("AWS_EVECUTION_ENV", null);
        }
    }
}