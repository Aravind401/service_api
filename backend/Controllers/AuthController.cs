using Microsoft.AspNetCore.Mvc;
using ServiceApi.DTOs;
using ServiceApi.Models;

namespace ServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<EmployeeCreateRequest> Login([FromBody] EmployeeCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserType) ||
            (request.UserType.ToLowerInvariant() is not "admin" and not "user"))
        {
            return BadRequest("UserType must be admin or user.");
        }

        if (string.IsNullOrWhiteSpace(request.EmployeeName) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("EmployeeName and Email are required.");

        if (request.UserType.Equals("admin", StringComparison.OrdinalIgnoreCase) && request.Salary is null)
            return BadRequest("Salary is required for admin user.");

        if (request.UserType.Equals("user", StringComparison.OrdinalIgnoreCase))
            request.Salary = null;

        return Ok(request);
    }
}
