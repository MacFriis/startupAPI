using StartupApi.Infrastructure;

namespace StartupApi.Model
{
    public class AppData : Resource
    {
        [Sortable]
        [Searchable]
        public string Category { get; set; }

        [Sortable]
        [Searchable]
        public string Title { get; set; }

        [Sortable]
        [Searchable]
        public string Subtitle { get; set; }
    }
}