using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CandidateController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _context = appDbContext;
            _environment = environment;
        }

        /*[HttpGet("getCandidates")]
        public IActionResult Get()
        {
            var candidates = _context.Candidates.ToList();
            return Ok(candidates);
        }*/

        [HttpGet("getCandidates")]
        public IActionResult GetCandidates(int pageSize = 5)
        {
            var candidates = _context.Candidates.Take(pageSize).ToList();
            return Ok(candidates);
        }

        [HttpGet("getRecruiterCandidates/{id}")]
        public IActionResult GetRecruiterCandidates(int id, int pageSize = 5)
        {
            var offers = _context.Offers.Where(o => o.idRec == id).Select(o => o.idOffer);
            var candidatures = _context.Candidatures.Where(c => offers.Contains(c.idOffer)).Select(c => c.idCand);
            var candidates = _context.Candidates.Where(c => candidatures.Contains(c.idCand)).Take(pageSize).ToList();

            return Ok(candidates);
        }

        [HttpGet("getAllRecruiterCandidates/{id}")]
        public IActionResult GetAllRecruiterCandidates(int id)
        {
            var offers = _context.Offers.Where(o => o.idRec == id).Select(o => o.idOffer);
            var candidatures = _context.Candidatures.Where(c => offers.Contains(c.idOffer)).Select(c => c.idCand);
            var candidates = _context.Candidates.Where(c => candidatures.Contains(c.idCand)).ToList();

            return Ok(candidates);
        }

        [HttpGet("getAllCandidates")]
        public IActionResult getAllCandidates()
        {
            var candidates = _context.Candidates.ToList();
            return Ok(candidates);
        }

        [HttpGet("getCandidate/{id}")]
        public async Task<ActionResult<Candidate>> Get(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return candidate;
        }

        [HttpDelete("deleteCandidate/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            string deleteImageFromFolder = Path.Combine(_environment.WebRootPath, "Content/Candidate/Images");
            string deleteLMFromFolder = Path.Combine(_environment.WebRootPath, "Content/Candidate/LMs");
            string deleteCVFromFolder = Path.Combine(_environment.WebRootPath, "Content/Candidate/CVs");

            string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteImageFromFolder, candidate.candImagePath);
            string currentLM = Path.Combine(Directory.GetCurrentDirectory(), deleteLMFromFolder, candidate.lmPath);
            string currentCV = Path.Combine(Directory.GetCurrentDirectory(), deleteCVFromFolder, candidate.cvPath);

            if (currentImage != null)
            {
                if (System.IO.File.Exists(currentImage))
                {
                    System.IO.File.Delete(currentImage);
                }
            }
            if (currentLM != null)
            {
                if (System.IO.File.Exists(currentLM))
                {
                    System.IO.File.Delete(currentLM);
                }
            }
            if (currentCV != null)
            {
                if (System.IO.File.Exists(currentCV))
                {
                    System.IO.File.Delete(currentCV);
                }
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("editCandidate/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Candidate candidate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   // var data = _context.Candidates.Where(e => e.idCand == id).SingleOrDefault();
                    var data = await _context.Candidates.FindAsync(id);

                    if (data == null)
                    {
                        return NotFound();
                    }

                    string uniqueImageName = string.Empty;
                    string uniqueLMName = string.Empty;
                    string uniqueCVName = string.Empty;

                    if (candidate.candImage != null)
                    {
                        if (!string.IsNullOrEmpty(data.candImagePath))
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "Content/Candidate/Images", data.candImagePath);

                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueImageName = UploadImage(candidate);
                    }

                    if (candidate.lmFile != null)
                    {
                        if (data.lmPath != null)
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "Content/Candidate/LMs", data.lmPath);

                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueLMName = UploadLM(candidate);
                    }

                    if (candidate.cvFile != null)
                    {
                        if (data.cvPath != null)
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "Content/Candidate/CVs", data.cvPath);

                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueCVName = UploadCV(candidate);
                    }

                    data.lName = candidate.lName;
                    data.fName = candidate.fName;
                    data.email = candidate.email;
                    data.age = candidate.age;
                    data.phone = candidate.phone;
                    data.email = candidate.email;
                    data.address = candidate.address;
                    data.cin = candidate.cin;
                    data.studyDegree = candidate.studyDegree;
                    data.diploma = candidate.diploma;
                    data.spec = candidate.spec;
                    data.expYears = candidate.expYears;

                    if (candidate.candImage != null)
                    {
                        data.candImagePath = uniqueImageName;
                    }
                    if (candidate.lmFile != null)
                    {
                        data.lmPath = uniqueLMName;
                    }
                    if (candidate.cvFile != null)
                    {
                        data.cvPath = uniqueCVName;
                    }
                    _context.Candidates.Update(data);
                    await _context.SaveChangesAsync();
                    return Ok(new { candidate });
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

        private string UploadImage(Candidate candidate)
        {
            string uniqueImageName = string.Empty;
            string filePath = string.Empty;

            if (candidate.candImage != null)
            {
                var ext = Path.GetExtension(candidate.candImage.FileName);
                var allowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only .png, .jpg and .jpeg extensions are allowed");
                    Console.WriteLine("Only .png, .jpg and .jpeg extensions are allowed");
                }
                else
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content", "Candidate", "Images");
                    uniqueImageName = Guid.NewGuid().ToString() + "_" + candidate.candImage.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueImageName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        candidate.candImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueImageName;
        }

        private string UploadLM(Candidate candidate)
        {
            string uniqueLMName = string.Empty;
            string filePath = string.Empty;

            if (candidate.lmFile != null)
            {
                var ext = Path.GetExtension(candidate.lmFile.FileName);
                var allowedExtensions = new string[] { ".pdf" };
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only .pdf extension is allowed");
                    Console.WriteLine("Only .pdf extensions is allowed");
                }
                else
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content", "Candidate", "LMs");
                    uniqueLMName = Guid.NewGuid().ToString() + "_" + candidate.lmFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueLMName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        candidate.lmFile.CopyTo(fileStream);
                    }
                }
            }
            return uniqueLMName;
        }

        private string UploadCV(Candidate candidate)
        {
            string uniqueCVName = string.Empty;
            string filePath = string.Empty;

            if (candidate.cvFile != null)
            {
                var ext = Path.GetExtension(candidate.cvFile.FileName);
                var allowedExtensions = new string[] { ".pdf" };
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Only .pdf extension is allowed");
                    Console.WriteLine("Only .pdf extensions is allowed");
                }
                else
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content", "Candidate", "CVs");
                    uniqueCVName = Guid.NewGuid().ToString() + "_" + candidate.cvFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueCVName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        candidate.cvFile.CopyTo(fileStream);
                    }
                }
            }
            return uniqueCVName;
        }
    }
}
