using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Context;
using RecruitingAPI.Migrations;
using RecruitingAPI.Models;
using System.Data;
using System.Reflection;
using System.Security.Claims;
using Offer = RecruitingAPI.Models.Offer;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OfferController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpPost("addOffer")]
        public async Task<IActionResult> Post([FromForm] Offer offer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = new Offer()
                    {
                        title = offer.title,
                        diploma = offer.diploma,
                        studyDegree = offer.studyDegree,
                        businessSector = offer.businessSector,
                        expYears = offer.expYears,
                        contractType = offer.contractType,
                        city = offer.city,
                        availability = offer.availability,
                        hiredNum = offer.hiredNum,
                        salary = offer.salary,
                        description = offer.description,
                        missions = offer.missions,
                        skills = offer.skills,
                        languages = offer.languages,
                        pubDate = offer.pubDate,
                        endDate = offer.endDate,
                        idRec = offer.idRec
                    };
                    _context.Offers.Add(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Offer added" });
                }
                ModelState.AddModelError(string.Empty, "Model property is not valid");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return BadRequest(new { Message = "Failed to add offer" });
        }

        [HttpGet("getAllOffers")]
        public IActionResult GetAllOffers()
        {
            var offers = _context.Offers.Include(o => o.Recruiter).ToList();
            return Ok(offers);
        }

        /*  [HttpGet("getOffers/{idRec}")]
          public IActionResult GetOffers(int idRec)
          {
              var offers = _context.Offers.Where(o => o.idRec == idRec).ToList();
              return Ok(offers);
          }*/


        [HttpGet("getOffers")]
        [Authorize]
        public IActionResult Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdInt = int.Parse(userId);

            var offers = _context.Offers.Include(o => o.Recruiter).Where(o => o.idRec == userIdInt).ToList();
            return Ok(offers);
        }


        [HttpGet("getOffer/{id}")]
        public async Task<ActionResult<Offer>> Get(int id)
        {
            var offer = await _context.Offers.Include(o => o.Recruiter).FirstOrDefaultAsync(o => o.idOffer == id);

            if (offer == null)
            {
                return NotFound();
            }

            return offer;
        }

        [HttpDelete("deleteOffer/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("editOffer/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] Offer offer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Offers.Where(e => e.idOffer == id).SingleOrDefault();

                    data.title = offer.title;
                    data.diploma = offer.diploma;
                    data.studyDegree = offer.studyDegree;
                    data.businessSector = offer.businessSector;
                    data.expYears = offer.expYears;
                    data.contractType = offer.contractType;
                    data.city = offer.city;
                    data.availability = offer.availability;
                    data.hiredNum = offer.hiredNum;
                    data.salary = offer.salary;
                    data.description = offer.description;
                    data.missions = offer.missions;
                    data.skills = offer.skills;
                    data.languages = offer.languages;
                    data.pubDate = offer.pubDate;
                    data.endDate = offer.endDate;
                    data.idRec = offer.idRec;

                    _context.Offers.Update(data);
                    _context.SaveChanges();
                    return Ok(new { Message = "Offer updated" });
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
