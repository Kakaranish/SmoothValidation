using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmoothValidation.DependencyInjection.Filters;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpPost("test")]
        [Validate]
        public IActionResult Test([FromBody] User user)
        {
            return Ok();
        }
    }
}
