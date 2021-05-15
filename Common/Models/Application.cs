
namespace Common.Models
{
    using Newtonsoft.Json;

    public class Application
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

    }
}