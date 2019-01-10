using System;
using Newtonsoft.Json;

namespace StartupApi.Model
{
    public abstract class Resource : Link
    {
        [JsonIgnore]
        public Link Self { get; set; }
    }
}
