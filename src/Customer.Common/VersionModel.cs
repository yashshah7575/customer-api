namespace Customer.Common
{
    public class VersionModel
    {
        public string BuildVersion { get; set; }

        public string MachineName { get; set; }

        public string Environment { get; set; }

        public string AwsRegion { get; set; }

        public string DefaultAwsRegion { get; set; }

        public string AwsExecutionEnvironment { get; set; }
    }
}