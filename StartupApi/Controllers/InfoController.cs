using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StartupApi.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StartupApi.Controllers
{
    [Route("/[controller]")]
    public class InfoController : Controller
    {
        private readonly ApiInfo apiInfo;

        public InfoController(IOptions<ApiInfo> apiInfoWrapper)
        {
            apiInfo = apiInfoWrapper.Value;
        }

        [HttpGet(Name = nameof(GetInfo))]
        [ProducesResponseType(200)]
        public ActionResult<ApiInfo> GetInfo()
        {
            return apiInfo;
        }
    }
}
