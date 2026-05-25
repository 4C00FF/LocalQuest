using System.Text.Json.Serialization;

namespace LocalQuest.Models._2020
{
    public class TokenResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; } = null;

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; } = null;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("key")]
        public string Key { get; set; } = "MDdmOWU3ZmUtOTE2OC00Njg0LTk0MDYtN2I2NjEyYjg4MmVm";
    }
}
