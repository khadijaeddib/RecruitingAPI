using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Net.Http.Headers;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CompanyController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("getCompanies")]
        public IActionResult Get()
        {
            var companies = _context.Companies.ToList();
            return Ok(companies);
        }

        [HttpPost("addCompany")]
        public async Task<IActionResult> Post([FromForm] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var ext = Path.GetExtension(company.logoImage.FileName);
                    var allowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };

                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError(string.Empty, "Only .png, .jpg, .jpeg extensions are allowed");
                        return BadRequest(new { Message = "Failed to add company; Only .png, .jpg, .jpeg extensions are allowed" });
                    }

                    string uniqueFileName = UploadImage(company);
                    var data = new Company()
                    {
                        logoPath = uniqueFileName,
                        name = company.name,
                        website = company.website,
                        businessSector = company.businessSector,
                        description = company.description,
                        phone = company.phone,
                        email = company.email,
                        address = company.address,
                        rc = company.rc,
                        idF = company.idF,
                        ice = company.ice,
                        legalStatus = company.legalStatus

                    };
                    _context.Companies.Add(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Company added" });
                }
                ModelState.AddModelError(string.Empty, "Model property is not valid");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return BadRequest(new { Message = "Failed to add company" });
        }

        private string UploadImage(Company company)
        {
            string uniqueFileName = string.Empty;
            string filePath = string.Empty;

            if (company.logoImage != null)
            {

                var ext = Path.GetExtension(company.logoImage.FileName);
                var allowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only .png, .jpg, .jpeg extensions are allowed");
                    Console.WriteLine("Only .png extensions is allowed");
                }else
                { 
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content","Company");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + company.logoImage.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        company.logoImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }

        [HttpDelete("deleteCompany/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            string deleteFromFolder = Path.Combine(_environment.WebRootPath, "Content/Company");
            string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFromFolder, company.logoPath);
            if (currentImage != null)
            {
                if (System.IO.File.Exists(currentImage))
                {
                    System.IO.File.Delete(currentImage);
                }
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getCompany/{id}")]
        public async Task<ActionResult<Company>> Get(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPost("editCompany/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Companies.Where(e => e.idCo == id).SingleOrDefault();
                    string uniqueFileName = string.Empty;
                    if (company.logoImage != null)
                    {
                        if (data.logoPath != null)
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "Content/Company", data.logoPath);

                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueFileName = UploadImage(company);
                    }

                    data.name = company.name;
                    data.website = company.website;
                    data.businessSector = company.businessSector;
                    data.description = company.description;
                    data.phone = company.phone;
                    data.email = company.email;
                    data.address = company.address;
                    data.rc = company.rc;
                    data.idF = company.idF;
                    data.ice = company.ice;
                    data.legalStatus = company.legalStatus;

                    if (company.logoImage != null)
                    {
                        data.logoPath = uniqueFileName;
                    }
                    _context.Companies.Update(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Company updated" });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

    }
}
