using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Models;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatureController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CandidatureController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpPost("addCandidature")]
        public async Task<IActionResult> Post([FromForm] Candidature candidature)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = new Candidature()
                    {
                        status = candidature.status,
                        dateCand = candidature.dateCand,
                        motivation = candidature.motivation,
                        idCand = candidature.idCand,
                        idOffer = candidature.idOffer
                    };
                    _context.Candidatures.Add(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Candidature added" });
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

        [HttpGet("getAllCandidatures")]
        public IActionResult GetAllCandidatures(int pageSize = 5)
        {
                var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).Take(pageSize).ToList();
                return Ok(candidatures);
        }

        [HttpGet("getCandidatures")]
        public IActionResult getCandidatures()
        {
            var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).ToList();
            return Ok(candidatures);
        }

        [HttpGet("getCandidateCandidatures/{id}")]
        public IActionResult GetCandidateCandidatures(int id, int pageSize = 5)
        {
            var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).Where(c => c.idCand == id).Take(pageSize).ToList();

            return Ok(candidatures);
        }

        [HttpGet("getAllCandidateCandidatures/{id}")]
        public IActionResult GetAllCandidateCandidatures(int id)
        {
            var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).Where(c => c.idCand == id).ToList();

            return Ok(candidatures);
        }

        [HttpGet("getRecruiterCandidatures/{id}")]
        public IActionResult GetRecruiterCandidatures(int id, int pageSize = 5)
        {
            var offers = _context.Offers.Where(o => o.idRec == id).Select(o => o.idOffer);
            var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).Where(c => offers.Contains(c.idOffer)).Take(pageSize).ToList();

            return Ok(candidatures);
        }

        [HttpGet("getAllRecruiterCandidatures/{id}")]
        public IActionResult GetAllRecruiterCandidatures(int id)
        {
            var offers = _context.Offers.Where(o => o.idRec == id).Select(o => o.idOffer);
            var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).Where(c => offers.Contains(c.idOffer)).ToList();

            return Ok(candidatures);
        }

        [HttpGet("getCandidature/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var candidature = await _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).FirstOrDefaultAsync(c => c.idCandidature == id);
                if (candidature == null)
                {
                    return NotFound(new { Message = "Candidature not found" });
                }
                return Ok(candidature);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Message = "Failed to get candidature" });
            }
        }

        [HttpGet("hasApplied/{candidateId}/{offerId}")]
        public IActionResult HasApplied(int candidateId, int offerId)
        {
            var hasApplied = _context.Candidatures.Any(c => c.idCand == candidateId && c.idOffer == offerId);
            return Ok(hasApplied);
        }

        [HttpDelete("deleteCandidature/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var interviews = _context.Interviews.Where(i => i.idCandidature == id);
                _context.Interviews.RemoveRange(interviews);
                await _context.SaveChangesAsync();

                var candidature = await _context.Candidatures.FindAsync(id);
                if (candidature == null)
                {
                    return NotFound(new { Message = "Candidature not found" });
                }
                _context.Candidatures.Remove(candidature);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Candidature deleted" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Message = "Failed to delete candidature" });
            }
        }

        [HttpPost("editCandidature/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Candidature candidature)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.Candidatures.FindAsync(id);
                    if (data == null)
                    {
                        return NotFound(new { Message = "Candidature not found" });
                    }
                    data.status = candidature.status;
                    _context.Candidatures.Update(data);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Candidature updated" });
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
