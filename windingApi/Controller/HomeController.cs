using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace windingApi.Controller;


[ApiController]
[Route("api/[controller]")]
public class HomeController : Microsoft.AspNetCore.Mvc.Controller
{
    // GET
    [HttpGet("just")]
    public ActionResult Index()
    {
        return Ok("working");
    }
}