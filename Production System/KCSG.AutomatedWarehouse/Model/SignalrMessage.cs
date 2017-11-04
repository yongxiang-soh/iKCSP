using KCSG.Core.Models;
using Newtonsoft.Json;

namespace KCSG.AutomatedWarehouse.Model
{
    public class SignalrMessage
    {
        [JsonProperty("message")]
        public SignalrMessageViewModel Message { get; set; }
    }
}