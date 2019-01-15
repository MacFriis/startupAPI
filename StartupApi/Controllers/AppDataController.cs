using System;
using Microsoft.AspNetCore.Mvc;

namespace StartupApi.Controllers
{

    [Route("/[controller]")]
    [ApiController]
    public class AppDataController : ControllerBase
    {
        internal static object GetDataById()
        {
            throw new NotImplementedException();
        }
    }
}