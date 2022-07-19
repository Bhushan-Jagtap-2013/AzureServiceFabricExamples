using Newtonsoft.Json;

namespace ECommerce.WebApi.Model
{
    public class ApiProduct
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }    
    }
}
