using System.Text.Json.Serialization;

namespace Customer.Common
{
    public class ApiResponseData<T>
    {
        [JsonPropertyName("data")]
		public T Data { get; set; }
    }
}