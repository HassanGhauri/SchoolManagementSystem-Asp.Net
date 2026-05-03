using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Api.Data;
using SchoolManagementSystem.Api.Models;

namespace SchoolManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("sms")]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // GET: sms/subjects
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();
            return Ok(subjects);
        }

        // GET: sms/subject/1
        [HttpGet("subject/{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Classes)
                    .ThenInclude(cs => cs.Class)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return NotFound("Subject not found");

            return Ok(subject);
        }

        // POST: sms/subject
        [HttpPost("subject")]
        public async Task<IActionResult> AddSubject([FromBody] Subject subject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Subject Added Successfully",
                data = subject
            });
        }

        // PUT: sms/subject/1
        [HttpPut("subject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] Subject updatedSubject)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
                return NotFound("Subject not found");

            subject.Name = updatedSubject.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Subject {id} Updated Successfully",
                data = subject
            });
        }

        // DELETE: sms/subject/1
        [HttpDelete("subject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
                return NotFound("Subject not found");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return Ok($"Subject {id} Deleted Successfully");
        }
    }
}