using Microsoft.AspNetCore.Mvc;

namespace Web2PnK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public async Task<IActionResult> Index()
        {
            return Ok(new { Title = "Домашня сторінка" });
        }

    }
}
