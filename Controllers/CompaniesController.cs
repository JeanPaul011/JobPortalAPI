using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace JobPortalAPI.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public CompaniesController(JobPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.Include(c => c.Jobs).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.Include(c => c.Jobs).FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return company;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] Company company)
        {
            if (company == null)
            {
                return BadRequest("Company data is required.");
            }

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }


        [HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteCompany(int id)
{
    Console.WriteLine($"🔹 DELETE request received for Company ID: {id}");

    var company = await _context.Companies
        .AsTracking()
        .FirstOrDefaultAsync(c => c.Id == id);

    if (company == null)
    {
        Console.WriteLine($"🔴 ERROR: Company with ID {id} not found.");
        return NotFound();
    }

    Console.WriteLine($"✅ Company with ID {id} found. Proceeding with deletion.");

    _context.Companies.Remove(company);
    await _context.SaveChangesAsync();

    Console.WriteLine($"✅ Company with ID {id} deleted successfully.");
    return NoContent();
}



    }
}
