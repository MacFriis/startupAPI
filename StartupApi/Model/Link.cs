using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace StartupApi.Model
{
    // This class helps to make the link when return data on the REST api
    public class Link
    {
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";

        public static Link To(string routeName, object routeValues = null)
            => new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };

        public static Link ToCollection(string routeName, object routeValues = null)
            => new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = new[] { "collection" }

            };

        public static Link ToForm(
         string routeName,
         object routeValues = null,
         string method = PostMethod,
         params string[] relations)
         => new Link
         {
             RouteName = routeName,
             RouteValues = routeValues,
             Method = method,
             Relations = relations
         };


        [JsonProperty(Order = -4)]
        public string HRef { get; set; }

        [JsonProperty(Order = -3,
            PropertyName = "rel",
            NullValueHandling = NullValueHandling.Ignore)]
        public string[] Relations { get; set; }

        [JsonProperty(Order = -2,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(GetMethod)]
        public string Method { get; set; }

        [JsonIgnore]
        public string RouteName { get; set; }

        [JsonIgnore]
        public object RouteValues { get; set; }
    }
}
