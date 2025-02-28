using JobPortalAPI.Models;
using JobPortalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortalAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly JobPortalContext _context;

        public CompanyService(JobPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync()
        {
            return await _context.Companies
                .Select(c => new CompanyDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Industry = c.Industry,
                    Location = c.Location
                }).ToListAsync();
        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            return company == null ? null : new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Location = company.Location
            };
        }

        public async Task AddCompanyAsync(CompanyDTO companyDto)
        {
            var company = new Company
            {
                Name = companyDto.Name,
                Industry = companyDto.Industry,
                Location = companyDto.Location
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }
    }
}
