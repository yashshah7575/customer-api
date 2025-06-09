using Customer.Common;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    [ApiController]
    [Route("api/system")]
    [Produces("application/json")]
    [ApiVersionNeutral]
    public class SystemController : Controller
    {
        private const string VersionFileName = "version.txt";
        private const string Unknown = "unknown";
        public readonly ILogger _logger;
        public readonly IWebHostEnvironment _hostingEnvironment;

        public SystemController(IWebHostEnvironment hostingEnvironment, ILogger<SystemController> logger)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("ping")]
        public IActionResult Index()
        {
            return Ok("pong");
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            _logger.LogInformation("Test logging");
            string fileLocation = Path.Combine(_hostingEnvironment.WebRootPath ??
                _hostingEnvironment.ContentRootPath, VersionFileName);
            string versionInfo = Unknown;

            if (System.IO.File.Exists(fileLocation))
            {
                versionInfo = System.IO.File.ReadAllText(fileLocation).Replace("\n\n", string.Empty).Trim();
            }

            var versionModel = new VersionModel
            {
                BuildVersion = versionInfo,
                MachineName = Environment.MachineName,
                Environment = _hostingEnvironment.EnvironmentName,
                AwsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? Unknown,
                DefaultAwsRegion = Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION") ?? Unknown,
                AwsExecutionEnvironment = Environment.GetEnvironmentVariable("AWS_EVECUTION_ENV") ?? Unknown
            };

            return Ok(versionModel);
        }
    }
}