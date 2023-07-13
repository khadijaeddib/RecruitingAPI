using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitingAPI.Context;
using RecruitingAPI.Models;

namespace RecruitingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpPost("contact")]
        public async Task<IActionResult> Contact([FromForm] Contact contact)
        {
            // Create a new contact object
            var newContact = new Contact
            {
                name = contact.name,
                email = contact.email,
                subject = contact.subject,
                message = contact.message
            };

            try
            {
                // Add the contact to the database
                _context.Contact.Add(newContact);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to send message" });
            }
        }

        [HttpGet("getContacts")]
        public IActionResult Get(int pageSize = 5)
        {
            var contacts = _context.Contact.Take(pageSize).ToList();
            return Ok(contacts);
        }

        [HttpDelete("deleteContact/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
