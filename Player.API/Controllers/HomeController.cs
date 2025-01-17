using Microsoft.AspNetCore.Mvc;

namespace Player.API.Controllers;

public class HomeController : BaseController
{
    public HomeController(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}