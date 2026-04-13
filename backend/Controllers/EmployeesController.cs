using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceApi.Data;
using ServiceApi.DTOs;
using ServiceApi.Models;
using ServiceApi.Services;

namespace ServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(AppDbContext dbContext, CertificateService certificateService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EmployeeRecord>> CreateEmployee(EmployeeCreateRequest request)
    {
        var userType = request.UserType.ToLowerInvariant();
        if (userType is not ("admin" or "user"))
            return BadRequest("userType must be admin or user.");

        if (userType == "admin" && request.Salary is null)
            return BadRequest("salary is required for admin.");

        if (userType == "user")
            request.Salary = null;

        var record = new EmployeeRecord
        {
            UserType = userType,
            EmployeeName = request.EmployeeName,
            Age = request.Age,
            Dob = request.Dob,
            Email = request.Email,
            Salary = request.Salary
        };

        dbContext.EmployeeRecords.Add(record);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeRecord>>> GetAll([FromQuery] string? search)
    {
        var query = dbContext.EmployeeRecords.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.ToLowerInvariant();
            query = query.Where(e =>
                e.EmployeeName.ToLower().Contains(q) ||
                e.Email.ToLower().Contains(q) ||
                e.UserType.ToLower().Contains(q) ||
                e.CertificateCode.ToLower().Contains(q));
        }

        return Ok(await query.OrderByDescending(e => e.CreatedAtUtc).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeRecord>> GetById(int id)
    {
        var record = await dbContext.EmployeeRecords.FindAsync(id);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpGet("{id:int}/certificate/excel")]
    public async Task<IActionResult> ExportExcel(int id)
    {
        var record = await dbContext.EmployeeRecords.FindAsync(id);
        if (record is null) return NotFound();

        var content = certificateService.GenerateExcel(record);
        return File(content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"certificate-{record.CertificateCode}.xlsx");
    }

    [HttpGet("{id:int}/certificate/pdf")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var record = await dbContext.EmployeeRecords.FindAsync(id);
        if (record is null) return NotFound();

        var content = certificateService.GeneratePdf(record);
        return File(content, "application/pdf", $"certificate-{record.CertificateCode}.pdf");
    }

    [HttpGet("certificate/scan/{certificateCode}")]
    public async Task<ActionResult<EmployeeRecord>> Scan(string certificateCode)
    {
        var record = await dbContext.EmployeeRecords.FirstOrDefaultAsync(x => x.CertificateCode == certificateCode);
        if (record is null) return NotFound();

        dbContext.CertificateScanLogs.Add(new CertificateScanLog
        {
            EmployeeRecordId = record.Id,
            ScannerIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        await dbContext.SaveChangesAsync();
        return Ok(record);
    }
}
