using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.Net;
using System.Numerics;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RecruiterController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _context = appDbContext;
            _environment = environment;
        }

        [HttpGet("getRecruiters")]
        public IActionResult Get()
        {
            var recruiters = _context.Recruiters.Include(r => r.Company).ToList();
            return Ok(recruiters);
        }


        [HttpGet("getRecruiter/{id}")]
        public async Task<ActionResult<Recruiter>> Get(int id)
        {
            var recruiter = await _context.Recruiters.Include(r => r.Company).FirstOrDefaultAsync(r => r.idRec == id);

            if (recruiter == null)
            {
                return NotFound();
            }

            return recruiter;
        }


        [HttpDelete("deleteRecruiter/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var interviews = _context.Interviews.Where(i => i.Candidature.Offer.Recruiter.idRec == id);
            _context.Interviews.RemoveRange(interviews);
            await _context.SaveChangesAsync();
            
            var recruiter = await _context.Recruiters.FindAsync(id);
            if (recruiter == null)
            {
                return NotFound();
            }

            string deleteImageFromFolder = Path.Combine(_environment.WebRootPath, "Content/Recruiter");

            string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteImageFromFolder, recruiter.recImagePath);

            if (currentImage != null)
            {
                if (System.IO.File.Exists(currentImage))
                {
                    System.IO.File.Delete(currentImage);
                }
            }

            _context.Recruiters.Remove(recruiter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("editRecruiter/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Recruiter recruiter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Recruiters.Where(e => e.idRec == id).SingleOrDefault();
                    string uniqueFileName = string.Empty;
                    if (recruiter.recImage != null)
                    {
                        if (data.recImagePath != null)
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "Content/Recruiter", data.recImagePath);

                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueFileName = UploadImageRec(recruiter);
                    }

                    data.lName = recruiter.lName;
                    data.fName = recruiter.fName;
                    data.email = recruiter.email;
                    data.phone = recruiter.phone;
                    data.age = recruiter.age;
                    data.address = recruiter.address;
                    data.career = recruiter.career;
                    data.idCo = recruiter.idCo;

                    if (recruiter.recImage != null)
                    {
                        data.recImagePath = uniqueFileName;
                    }
                    _context.Recruiters.Update(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Recruiter updated" });
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

        private string UploadImageRec(Recruiter recruiter)
        {
            string uniqueImageName = string.Empty;
            string filePath = string.Empty;

            if (recruiter.recImage != null)
            {
                var ext = Path.GetExtension(recruiter.recImage.FileName);
                var allowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only .png, .jpg and .jpeg extensions are allowed");
                    Console.WriteLine("Only .png, .jpg and .jpeg extensions are allowed");
                }
                else
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content", "Recruiter");
                    uniqueImageName = Guid.NewGuid().ToString() + "_" + recruiter.recImage.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueImageName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        recruiter.recImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueImageName;
        }
    }
}
