using System;
namespace StartupApi.Model
{
    public class UsersResponse : PagedCollection<User>
    {
        public Form Registre { get; set; }
        public Link Me { get; set; }
    }
}
