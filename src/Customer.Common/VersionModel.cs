namespace Customer.Common;

public class VersionModel
{
    public string BuildVersion { get; set; } = default!;

    public string MachineName { get; set; } = default!;

    public string Environment { get; set; } = default!;

    public string AwsRegion { get; set; } = default!;

    public string DefaultAwsRegion { get; set; } = default!;

    public string AwsExecutionEnvironment { get; set; } = default!;
}
