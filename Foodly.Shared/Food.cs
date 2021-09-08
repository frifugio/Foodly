using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Foodly.Shared
{
    public class Food
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }
        [JsonProperty("maxQuantity")]
        [Range(0, 14)]
        public int MaxQuantity { get; set; }
        [JsonProperty("actualQuantity")]
        [Range(0, 14)]
        public int ActualQuantity { get; set; }
        [JsonProperty("optionalQuantity")]
        [Range(0,14)]
        public int OptionalQuantity { get; set; }
        [JsonProperty("overQuantity")]
        [Range(0,14)]
        public int OverQuantity { get; set; }
    }
}
