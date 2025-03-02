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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return Ok(await _companyRepository.GetCompaniesWithJobsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<Company>> CreateCompany([FromBody] Company company)
        {
            await _companyRepository.AddAsync(company);
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
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
