using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.Text.RegularExpressions;

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
