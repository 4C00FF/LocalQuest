using System.Text.Json.Serialization;

namespace LocalQuest.Models._2020
{
    public class HeartbeatResponse
    {
        [JsonPropertyName("playerId")] public long PlayerId { get; set; }
        [JsonPropertyName("statusVisibility")] public int StatusVisibility { get; set; } = 0;
        [JsonPropertyName("deviceClass")] public int DeviceClass { get; set; } = 0;
        [JsonPropertyName("vrMovementMode")] public int VrMovementMode { get; set; } = 1;
        [JsonPropertyName("roomInstance")] public object? RoomInstance { get; set; } = null;
        [JsonPropertyName("isOnline")] public bool IsOnline { get; set; } = true;
        [JsonPropertyName("appVersion")] public string AppVersion { get; set; } = "20210804";
        [JsonPropertyName("errorCode")] public int ErrorCode { get; set; } = 0;
        [JsonPropertyName("lastOnline")] public string LastOnline { get; set; } = "0001-01-01T00:00:00";
        [JsonPropertyName("clientJoinData")] public string ClientJoinData { get; set; } = "{\"WelcomeMatName\":\"Welcome Mat\"}";
    }
}
