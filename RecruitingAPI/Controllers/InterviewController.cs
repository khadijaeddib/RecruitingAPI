using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using RecruitingAPI.Context;
using RecruitingAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

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
        public async Task<IActionResult> Post([FromForm] Interview interview)
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
