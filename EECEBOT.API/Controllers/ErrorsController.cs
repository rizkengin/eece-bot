using Microsoft.AspNetCore.Mvc;

namespace EECEBOT.API.Controllers;

public class ErrorsController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    [HttpGet]
    public IActionResult Error() => Problem();
}