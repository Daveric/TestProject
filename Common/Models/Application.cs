
namespace Common.Models
{
    using System;
    using Newtonsoft.Json;

    public class Application
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonProperty("guid")]
        public Guid Stock { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

    }
}
