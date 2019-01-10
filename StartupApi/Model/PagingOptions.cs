using System;
using System.ComponentModel.DataAnnotations;

namespace StartupApi.Model
{
    public class PagingOptions
    {
        [Range(1, 999999, ErrorMessage = "Offset must be greater thatn 0 and less thatn 1.000.000")]
        public int? Offset { get; set; }

        [Range(1, 100, ErrorMessage = "Limit must be greater thatn 0 and less thatn 100")]
        public int? Limit { get; set; }

        public PagingOptions Replace(PagingOptions newer)
        {
            return new PagingOptions
            {
                Offset = newer.Offset ?? this.Offset,
                Limit = newer.Limit ?? this.Limit
            };
        }
    }
}
