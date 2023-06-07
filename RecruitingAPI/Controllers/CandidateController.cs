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
        public IActionResult Get(int pageSize = 5)
        {
            var candidates = _context.Candidates.Take(pageSize).ToList();
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
                    var data = _context.Candidates.Where(e => e.idCand == id).SingleOrDefault();

                    string uniqueImageName = string.Empty;
                    string uniqueLMName = string.Empty;
                    string uniqueCVName = string.Empty;

                    if (candidate.candImage != null)
                    {
                        if (data.candImagePath != null)
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
                    _context.SaveChanges();
                    return Ok(new { Message = "Candidate updated" });
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


        /*[HttpPost("register")]
        public async Task<ActionResult<Candidate>> Register(Candidate candidate)
        {
            string ConfirmPass = Request.Form["ConfirmPass"];
           *//* IFormFile ImageCandPath = Request.Form.Files.GetFile("ImageCandPath");
            IFormFile CVPath = Request.Form.Files.GetFile("CVPath");
            IFormFile LMPath = Request.Form.Files.GetFile("LMPath");*//*

            // Vérifiez que l'adresse e-mail n'est pas déjà enregistrée
            var existingCandidate = await _context.candidates.FirstOrDefaultAsync(c => c.Email == candidate.Email);
            if (existingCandidate != null)
            {
                ModelState.AddModelError("Email", "L'adresse e-mail est déjà enregistrée.");
            }

            // Vérifiez que le mot de passe et la confirmation du mot de passe sont identiques
            if (candidate.Pass != ConfirmPass)
            {
                ModelState.AddModelError("ConfirmPass", "Les mots de passe ne correspondent pas.");
            }

            // Vérifiez que les fichiers téléchargés sont valides
            if (string.IsNullOrEmpty(candidate.ImageCandPath) ||
                (!candidate.ImageCandPath.EndsWith(".jpg") && !candidate.ImageCandPath.EndsWith(".jpeg")))
            {
                ModelState.AddModelError("ImageCandPath", "L'image doit être au format .jpg ou .jpeg.");
            }

            if (string.IsNullOrEmpty(candidate.CVPath) || !candidate.CVPath.EndsWith(".pdf"))
            {
                ModelState.AddModelError("CVPath", "Le CV doit être au format .pdf.");
            }

            if (string.IsNullOrEmpty(candidate.LMPath) || !candidate.LMPath.EndsWith(".pdf"))
            {
                ModelState.AddModelError("LMPath", "La lettre de motivation doit être au format .pdf.");
            }

            // Vérifiez que le numéro de téléphone est valide
            if (!Regex.IsMatch(candidate.Phone, @"^[0-9 ]+$") || candidate.Phone.Length < 10 || candidate.Phone.Length > 20)
            {
                ModelState.AddModelError("Phone", "Le numéro de téléphone n'est pas valide.");
            }

            // Vérifiez que les champs lName et fName sont du texte
            if (!Regex.IsMatch(candidate.LName, @"^[a-zA-Z ]+$"))
            {
                ModelState.AddModelError("LName", "Le nom doit contenir uniquement des lettres et des espaces.");
            }

            if (!Regex.IsMatch(candidate.FName, @"^[a-zA-Z ]+$"))
            {
                ModelState.AddModelError("FName", "Le prénom doit contenir uniquement des lettres et des espaces.");
            }

            // Renvoyez une réponse d'erreur si les informations du candidat ne sont pas valides
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Enregistrez les fichiers téléchargés dans un dossier de votre projet
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            *//*if (ImageCandPath != null && ImageCandPath.Length > 0)
            {
                var imageCandFilePath = Path.Combine(uploadsFolder, ImageCandPath.FileName);
                using (var stream = new FileStream(imageCandFilePath, FileMode.Create))
                {
                    await ImageCandPath.CopyToAsync(stream);
                }
                candidate.ImageCandPath = imageCandFilePath;
            }

            if (CVPath != null && CVPath.Length > 0)
            {
                var cvFilePath = Path.Combine(uploadsFolder, CVPath.FileName);
                using (var stream = new FileStream(cvFilePath, FileMode.Create))
                {
                    await CVPath.CopyToAsync(stream);
                }
                candidate.CVPath = cvFilePath;
            }

            if (LMPath != null && LMPath.Length > 0)
            {
                var lmFilePath = Path.Combine(uploadsFolder, LMPath.FileName);
                using (var stream = new FileStream(lmFilePath, FileMode.Create))
                {
                    await LMPath.CopyToAsync(stream);
                }
                candidate.LMPath = lmFilePath;
            }*//*


            // Enregistrez le candidat dans la base de données
            _context.candidates.Add(candidate);
            await _context.SaveChangesAsync();

            // Renvoyez une réponse appropriée
            //return CreatedAtAction(nameof(GetCandidate), new { id = candidate.IdCand }, candidate);
            return Ok(new { Message = "Candidat est enregisté" });
        }*/

    }

}
