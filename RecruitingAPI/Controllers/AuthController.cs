using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using System.Security.Cryptography;
using BCrypt.Net;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MimeKit;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AuthController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] Login login)
        {
            // Check if email exists in the database
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.email == login.email);
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.email == login.email);
            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.email == login.email);

            if (candidate == null && admin == null && recruiter == null)
            {
                return BadRequest(new { Message = "Invalid email ..." });
            }

            // Verify password and generate token
            string role = null;
            object user = null;

            if (candidate != null && PasswordHasher.Validate(login.pwd, candidate.pass))
            {
                role = candidate.role;
                user = candidate;
            }
            else if (admin != null && PasswordHasher.Validate(login.pwd, admin.pass))
            {
                role = admin.role;
                user = admin;
            }
            else if (recruiter != null && PasswordHasher.Validate(login.pwd, recruiter.pass))
            {
                role = recruiter.role;
                user = recruiter;
            }

            if (user != null)
            {
               /* byte[] keyBytes = new byte[32]; // 32 bytes = 256 bits
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(keyBytes);
                }
                string key = Convert.ToBase64String(keyBytes);*/
                // Generate JWT token
                var token = GenerateJwtToken(user, role);
                
                return Ok(new { Message = $"{role} login successfully", Token = token , role,  user});
            }else
            {
                return BadRequest(new { Message = "Invalid email or password" });
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPassword resetPassword)
        {
            // Check if email exists in the database
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.email == resetPassword.email);
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.email == resetPassword.email);
            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.email == resetPassword.email);

            if (candidate == null && admin == null && recruiter == null)
            {
                return BadRequest(new { Message = "Invalid email ..." });
            }

            // Verify that the new password and confirm password match
            if (resetPassword.newPassword != resetPassword.confirmNewPassword)
            {
                return BadRequest(new { Message = "Passwords do not match" });
            }

            // Update the password in the database
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(resetPassword.newPassword, salt);

            if (candidate != null)
            {
                candidate.pass = hashedNewPassword;
                _context.Candidates.Update(candidate);
            }
            else if (admin != null)
            {
                admin.pass = hashedNewPassword;
                _context.Admins.Update(admin);
            }
            else if (recruiter != null)
            {
                recruiter.pass = hashedNewPassword;
                _context.Recruiters.Update(recruiter);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password reset successfully" });
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] Admin admin)
        {
            // Check if the email is already registered
            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.email == admin.email);
            if (existingAdmin != null)
            {
                return BadRequest(new { Message = "Email is already registered" });
            }
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Create a new admin object
            var newAdmin = new Admin
            {
                email = admin.email,
                pass = BCrypt.Net.BCrypt.HashPassword(admin.pass, salt)

            };

            try
            {
                // Add the admin to the database
                _context.Admins.Add(newAdmin);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Admin registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to register admin" });
            }
        }


        [HttpPost("register/candidate")]
        public async Task<IActionResult> RegisterCandidate([FromForm] Candidate candidate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the email is already registered
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(c => c.email == candidate.email);
                    if (existingCandidate != null)
                    {
                        return BadRequest(new { Message = "Email is already registered" });
                    }

                    // Vérifiez que le mot de passe et la confirmation du mot de passe sont identiques
                    if (candidate.pass != candidate.confirmPass)
                    {
                        return BadRequest(new { Message = "ConfirmPass not match" });
                    }

                    var imageExt = Path.GetExtension(candidate.candImage.FileName);
                    var LMExt = Path.GetExtension(candidate.lmFile.FileName);
                    var CVExt = Path.GetExtension(candidate.cvFile.FileName);

                    var imageAllowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };
                    var allowedExtensions = new string[] { ".pdf" };

                    if (!imageAllowedExtensions.Contains(imageExt))
                    {
                        ModelState.AddModelError(string.Empty, "Only .png, .jpg and .jpeg extensions are allowed");
                        return BadRequest(new { Message = "Failed to add candidate; Only .png, .jpg and .jpeg extensions are allowed" });
                    }

                    if (!allowedExtensions.Contains(LMExt) || !allowedExtensions.Contains(CVExt))
                    {
                        ModelState.AddModelError(string.Empty, "Only .pdf extensions is allowed");
                        return BadRequest(new { Message = "Failed to add candidate; Only.pdf extensions is allowed" });
                    }

                    string uniqueImageName = UploadImage(candidate);
                    string uniqueLMName = UploadLM(candidate);
                    string uniqueCVName = UploadCV(candidate);

                    string salt = BCrypt.Net.BCrypt.GenerateSalt();

                    // Create a new candidate object
                    var newCandidate = new Candidate
                    {
                        candImagePath = uniqueImageName,
                        lName = candidate.lName,
                        fName = candidate.fName,
                        email = candidate.email,
                        age = candidate.age,
                        phone = candidate.phone,
                        address = candidate.address,
                        cin = candidate.cin,
                        studyDegree = candidate.studyDegree,
                        diploma = candidate.diploma,
                        spec = candidate.spec,
                        expYears = candidate.expYears,
                        lmPath = uniqueLMName, 
                        cvPath = uniqueCVName, 
                        pass = BCrypt.Net.BCrypt.HashPassword(candidate.pass, salt),
                        token = string.Empty // Initialize the token to an empty string
                    };

                    // Add the candidate to the database
                    _context.Candidates.Add(newCandidate);
                    await _context.SaveChangesAsync();

                    var candidateName = newCandidate.fName + " " + newCandidate.lName;
                    // Send welcome email to the new candidate
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ultimate Solutions ERP", "ultimatesolutionserp.ma@gmail.com"));
                    message.To.Add(new MailboxAddress(newCandidate.fName, newCandidate.email));
                    message.Subject = "Bienvenue dans notre plateforme de recrutement";

                    var builder = new BodyBuilder();

                    // Get the path to the image file
                    var imagePath = Path.Combine(_environment.WebRootPath, "Content", "Company", "us_logo.png");

                    // Load the image from a file or URL
                    var image = builder.LinkedResources.Add(imagePath);
                    image.ContentId = MimeKit.Utils.MimeUtils.GenerateMessageId();

                    builder.HtmlBody = $"<div style=\"background-color: #eee; padding: 20px;\">" +
                                         $"<div style=\"text-align: left; width: 200px; height: 100px;\"><img src=\"cid:{image.ContentId}\" alt=\"Header Image\"></div>" +
                                         $"<h1>Bienvenue dans notre plateforme de recrutement</h1>" +
                                         $"<h3>Cher(e) {newCandidate.lName},</h3>" +
                                         $"<p>Nous vous souhaitons la bienvenue dans notre plateforme de recrutement. Nous sommes ravis de vous compter parmi nos candidats potentiels. Votre inscription est un premier pas vers de nombreuses opportunités professionnelles.</p>" +
                                         $"<p>N'hésitez pas à parcourir les offres d'emploi disponibles sur notre plateforme. Vous pouvez postuler aux offres qui correspondent à vos intérêts et à votre profil.</p>" +
                                         $"<p>Si vous avez des questions ou avez besoin d'assistance, n'hésitez pas à nous contacter. Notre équipe se tient à votre disposition pour vous aider dans votre recherche.</p>" +
                                         $"<p>Nous vous souhaitons beaucoup de succès dans votre parcours professionnel et nous espérons vous aider à trouver des opportunités passionnantes.</p>" +
                                         $"<p>Cordialement,</p>" +
                                         $"<p><i>L'équipe d'ULTIMATE SOLUTIONS ERP</i></p>" +
                                         "</div>";

                    message.Body = builder.ToMessageBody();

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        client.Authenticate("ultimatesolutionserp.ma@gmail.com", "qthoszbzgkslsdsu");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    return Ok(new { Message = "Candidate registered successfully" });
                }
                ModelState.AddModelError(string.Empty, "Model property is not valid");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            //return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to register candidate" });
            return BadRequest();

        }

        [HttpPost("register/recruiter")]
        public async Task<IActionResult> RegisterRecruiter([FromForm] Recruiter recruiter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the email is already registered
                    var existingRecruiter = await _context.Recruiters.FirstOrDefaultAsync(c => c.email == recruiter.email);
                    if (existingRecruiter != null)
                    {
                        return BadRequest(new { Message = "Email is already registered" });
                    }

                    var imageExt = Path.GetExtension(recruiter.recImage.FileName);

                    var imageAllowedExtensions = new string[] { ".png", ".jpg", ".jpeg" };

                    if (!imageAllowedExtensions.Contains(imageExt))
                    {
                        ModelState.AddModelError(string.Empty, "Only .png, .jpg and .jpeg extensions are allowed");
                        return BadRequest(new { Message = "Failed to add candidate; Only .png, .jpg and .jpeg extensions are allowed" });
                    }

                    string uniqueImageName = UploadImageRec(recruiter);

                    string salt = BCrypt.Net.BCrypt.GenerateSalt();

                    // Create a new candidate object
                    var newRecruiter = new Recruiter
                    {
                        recImagePath = uniqueImageName,
                        lName = recruiter.lName,
                        fName = recruiter.fName,
                        email = recruiter.email,
                        phone = recruiter.phone,
                        age = recruiter.age,
                        address = recruiter.address,
                        career = recruiter.career,
                        idCo = recruiter.idCo,
                        pass = BCrypt.Net.BCrypt.HashPassword(recruiter.pass, salt),
                        token = string.Empty // Initialize the token to an empty string
                    };

                    // Add the candidate to the database
                    _context.Recruiters.Add(newRecruiter);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Recruiter registered successfully" });
                }
                ModelState.AddModelError(string.Empty, "Model property is not valid");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            //return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to register candidate" });
            return BadRequest();

        }

        [HttpPost("incrementVisitorCounter")]
        public async Task<IActionResult> IncrementVisitorCounter()
        {
            var visitor = new Visitor { VisitDate = DateTime.Now };
            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("getTotalVisitors")]
        public async Task<IActionResult> GetTotalVisitors()
        {
            var totalVisitors = await _context.Visitors.CountAsync();
            return Ok(totalVisitors);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Invalidate the user's token here
            // ...

            return Ok(new { Message = "Logout successful" });
        }

        public class PasswordHasher
        {
            /*public static string Hash(string password, string salt)
            {
                
                return BCrypt.Net.BCrypt.HashPassword(password, salt);

                *//*var salt = RandomNumberGenerator.GetBytes(SaltSize);
                var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
                return string.Join(SaltDelimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));*//*
            }*/
            public static bool Validate(string password, string passwordHash)
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash );
            }
        }

        private string GenerateJwtToken(object user, string role)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Email, GetEmailFromUser(user)),
                    new Claim(ClaimTypes.NameIdentifier, GetIdFromUser(user))
                    // Add additional claims specific to the user type
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store the token in the database
            if (user is Candidate candidate)
            {
                candidate.token = tokenString;
            }
            else if (user is Admin admin)
            {
                admin.token = tokenString;
            }
            else if (user is Recruiter recruiter)
            {
                recruiter.token = tokenString;
            }

            _context.SaveChanges();
            return tokenString;
        }

        private string GetEmailFromUser(object user)
        {
            if (user is Candidate candidate)
            {
                return candidate.email;
            }
            else if (user is Admin admin)
            {
                return admin.email;
            }
            else if (user is Recruiter recruiter)
            {
                return recruiter.email;
            }

            return string.Empty;
        }

        private string GetIdFromUser(object user)
        {
            if (user is Candidate candidate)
            {
                return candidate.idCand.ToString();
            }
            else if (user is Admin admin)
            {
                return admin.idAdmin.ToString();
            }
            else if (user is Recruiter recruiter)
            {
                return recruiter.idRec.ToString();
            }
            return null;
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
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Content", "Candidate", "Images") ;
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