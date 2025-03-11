using Microsoft.AspNetCore.Mvc;
using JobPortalAPI.Models;
using JobPortalAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace JobPortalAPI.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyRepository companyRepository, ILogger<CompaniesController> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        // Everyone can view companies
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return Ok(await _companyRepository.GetCompaniesWithJobsAsync());
        }

        // Everyone can view specific company details
        [HttpGet("{id}")]

        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }
        // Update Company: PUT: api/companies/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company company)
        {
            _logger.LogInformation($"Updating company with ID {id}");

            if (id != company.Id)
            {
                _logger.LogWarning("Company ID mismatch");
                return BadRequest("ID in URL does not match ID in request body");
            }

            var exists = await _companyRepository.ExistsAsync(c => c.Id == id);
            if (!exists)
            {
                _logger.LogWarning($"Company with ID {id} not found for update");
                return NotFound();
            }

            try
            {
                await _companyRepository.UpdateAsync(company);
                _logger.LogInformation($"Company with ID {id} updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating company with ID {id}");
                return StatusCode(500, "An error occurred while updating the company");
            }
        }

        // Only Admins & Recruiters can create a company
        [HttpPost]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<Company>> CreateCompany([FromBody] Company company)
        {
            await _companyRepository.AddAsync(company);
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        // Only Admins can delete companies
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var exists = await _companyRepository.ExistsAsync(c => c.Id == id);
            if (!exists)
                return NotFound();

            await _companyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
