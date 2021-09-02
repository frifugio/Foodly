using Newtonsoft.Json;
using System;

namespace Foodly.Shared
{
    public class Food
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("maxQuantity")]
        public int MaxQuantity { get; set; }
        [JsonProperty("actualQuantity")]
        public int ActualQuantity { get; set; }
        [JsonProperty("optQuantity")]
        public int OptionalQuantity { get; set; }
    }
}
