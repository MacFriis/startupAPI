using System;
using Microsoft.AspNetCore.Mvc;

namespace StartupApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        internal static object GetUserById()
        {
            throw new NotImplementedException();
        }

        internal static object GetVisibleUsers()
        {
            throw new NotImplementedException();
        }
    }
}