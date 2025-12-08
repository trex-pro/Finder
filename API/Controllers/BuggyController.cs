using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("Bad Request.");
    }

    [HttpGet("auth-error")]
    public IActionResult GetAuth()
    {
        return Unauthorized();
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("Server Error.");
    }

    [Authorize(Roles="Admin")]
    [HttpGet("admin-secret")]
    public ActionResult<string> GetAdminSecret()
    {
        return Ok("Only Admin(s) Should See This.");
    }
}
