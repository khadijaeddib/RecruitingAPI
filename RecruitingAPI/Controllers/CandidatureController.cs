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
            }
            return BadRequest(new { Message = "Failed to add candidature" });
        }

        [HttpGet("getAllCandidatures")]
        public async Task<IActionResult> GetAllCandidatures()
        {
            try
            {
                var candidatures = _context.Candidatures.Include(c => c.Candidate).Include(c => c.Offer).ToList();
                return Ok(candidatures);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(new { Message = "Failed to get all candidatures" });
            }
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

        [HttpDelete("deleteCandidature/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
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
