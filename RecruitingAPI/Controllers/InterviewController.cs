using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {

        private readonly AppDbContext _context;

        public InterviewController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpPost("addInterview")]
        public async Task<IActionResult> Post([FromForm] Interview interview, [FromForm] string recruiterName, [FromForm] string recruiterEmail, [FromForm] string candidateName, [FromForm] string candidateEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = new Interview()
                    {
                        status = interview.status,
                        interviewDate = interview.interviewDate,
                        address = interview.address,
                        idCandidature = interview.idCandidature,
                        interviewFormat = interview.interviewFormat
                    };
                    _context.Interviews.Add(data);
                    _context.SaveChanges();

                    data = _context.Interviews.Include(i => i.Candidature).ThenInclude(c => c.Candidate).Include(i => i.Candidature).ThenInclude(c => c.Offer).ThenInclude(o => o.Recruiter).ThenInclude(r => r.Company).FirstOrDefault(i => i.idInterview == data.idInterview);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(recruiterName, recruiterEmail));
                    message.To.Add(new MailboxAddress(candidateName, candidateEmail));
                    message.Subject = "Confirmation d'entretien - Détails de l'entretien";


                    var builder = new BodyBuilder();
                    builder.HtmlBody = $"<h3>Cher(e) {data.Candidature.Candidate.lName},</h3><p>Nous sommes ravis de vous informer que votre entretien a été planifié avec succès pour le poste " +
                                         $"<b>{data.Candidature.Offer.title}</b> chez <b>{data.Candidature.Offer.Recruiter.Company.name}</b>. Veuillez trouver ci-dessous les détails de l'entretien :</p><hr>" +
                                         $"<h6>Date de l'entretien : {data.interviewDate}</h6>" +
                                         $"<h6>Format de l'entretien : {data.interviewFormat}</h6>" +
                                         $"<h6>Adresse ou lien : {data.address}</h6><hr>" +
                                         "<p>Nous vous recommandons de vous préparer en conséquence pour l'entretien en vous familiarisant avec l'entreprise et en révisant les compétences et les qualifications requises pour le poste.</p>" +
                                         $"<p>Si vous avez besoin de plus d'informations ou si vous rencontrez des problèmes, n'hésitez pas à nous contacter à l'adresse e-mail suivante : {data.Candidature.Offer.Recruiter.email}.</p>" +
                                         "<p>Cordialement,</p>";


                    message.Body = builder.ToMessageBody();

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        client.Authenticate("ultimatesolutionserp.ma@gmail.com", "qthoszbzgkslsdsu");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    return Ok(new { Message = "Interview added" });
                }
                ModelState.AddModelError(string.Empty, "Model property is not valid");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Error = ex.InnerException?.ToString() ?? ex.Message });
            }

            return BadRequest(new { Error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() });
        }

        [HttpGet("getAllInterviews")]
        public IActionResult getAllInterviews()
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).ToList();
            return Ok(interviews);
        }

        [HttpGet("getInterviews")]
        public IActionResult getInterviews(int pageSize = 5)
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).Take(pageSize).ToList();
            return Ok(interviews);
        }

        [HttpGet("getAllCandidateInterviews/{id}")]
        public IActionResult getAllCandidateInterviews(int id)
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).Where(i => i.Candidature.idCand == id).ToList();
            return Ok(interviews);
        }

        [HttpGet("getCandidateInterviews/{id}")]
        public IActionResult getCandidateInterviews(int id, int pageSize = 5)
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).Where(i => i.Candidature.idCand == id).Take(pageSize).ToList();

            return Ok(interviews);
        }

        [HttpGet("getAllRecruiterInterviews/{id}")]
        public IActionResult getAllRecruiterInterviews(int id)
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).ThenInclude(c => c.Offer).Where(i => i.Candidature.Offer.idRec == id).ToList();
            return Ok(interviews);
        }

        [HttpGet("getRecruiterInterviews/{id}")]
        public IActionResult getRecruiterInterviews(int id, int pageSize = 5)
        {
            var interviews = _context.Interviews.Include(i => i.Candidature).ThenInclude(c => c.Offer).Where(i => i.Candidature.Offer.idRec == id).Take(pageSize).ToList();

            return Ok(interviews);
        }

        [HttpGet("getInterview/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var interview = await _context.Interviews.Include(c => c.Candidature).FirstOrDefaultAsync(c => c.idInterview == id);
                if (interview == null)
                {
                    return NotFound(new { Message = "Interview not found" });
                }
                return Ok(interview);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Message = "Failed to get interview" });
            }
        }

        [HttpDelete("deleteInterview/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var interview = await _context.Interviews.FindAsync(id);
                if (interview == null)
                {
                    return NotFound(new { Message = "Interview not found" });
                }
                _context.Interviews.Remove(interview);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Interview deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Message = "Failed to delete interview" });
            }
        }

        [HttpPost("editInterview/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Interview interview)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.Interviews.FindAsync(id);
                    if (data == null)
                    {
                        return NotFound(new { Message = "Interview not found" });
                    }
                  /*  data.status = interview.status;*/
                    data.interviewDate = interview.interviewDate;
                    data.address = interview.address;
                    data.interviewFormat = interview.interviewFormat;
                    
                    _context.Interviews.Update(data);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Interview updated" });
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

        [HttpPost("editInterviewStatus/{id}")]
        public async Task<IActionResult> EditStatus(int id, [FromForm] Interview interview)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.Interviews.FindAsync(id);
                    if (data == null)
                    {
                        return NotFound(new { Message = "InterviewStatus not found" });
                    }
                    data.status = interview.status;
                    _context.Interviews.Update(data);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "InterviewStatus updated" });
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
