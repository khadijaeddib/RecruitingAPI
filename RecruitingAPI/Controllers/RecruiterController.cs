using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Models;

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

       /* [HttpGet("getRecruiters")]
        public IActionResult Get()
        {
            var recruiters = _context.Recruiters.ToList();
            return Ok(recruiters);
        }*/

        [HttpGet("getRecruiters")]
        public IActionResult Get()
        {
            var recruiters = _context.Recruiters.Include(r => r.Company).ToList();
            return Ok(recruiters);
        }


        /*[HttpGet("getRecruiter/{id}")]
        public async Task<ActionResult<Recruiter>> Get(int id)
        {
            var recruiter = await _context.Recruiters.FindAsync(id);

            if (recruiter == null)
            {
                return NotFound();
            }

            return recruiter;
        }*/

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
    }
}
