using System;
namespace StartupApi.Model
{
    public class AppDataEntity
    {
        public Guid Id { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }
    }
}
