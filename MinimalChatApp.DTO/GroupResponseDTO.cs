using System.Text.Json.Serialization;

namespace MinimalChatApp.DTO
{
    public class GroupResponseDTO
    {
        [JsonPropertyName("groupId")]
        public int Id { get; set; }

        [JsonIgnore]
        public int CreatorId { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<string> Members { get; set; } = new();

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}
