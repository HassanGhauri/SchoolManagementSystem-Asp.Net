using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using SchoolManagementSystem.Api.Data;
using SchoolManagementSystem.Api.Models;

namespace SchoolManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("sms")]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // GET SUBJECTS
        // =========================================

        [Authorize(Roles = "Principal,Teacher,Student")]
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();

            return Ok(subjects);
        }

        // =========================================
        // GET SUBJECT BY ID
        // =========================================

        [Authorize(Roles = "Principal,Teacher,Student")]
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

        // =========================================
        // ADD SUBJECT
        // Principal Only
        // =========================================

        [Authorize(Roles = "Principal")]
        [HttpPost("subject")]
        public async Task<IActionResult> AddSubject(
            [FromBody] Subject subject
        )
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

        // =========================================
        // UPDATE SUBJECT
        // Principal Only
        // =========================================

        [Authorize(Roles = "Principal")]
        [HttpPut("subject/{id}")]
        public async Task<IActionResult> UpdateSubject(
            int id,
            [FromBody] Subject updatedSubject
        )
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

        // =========================================
        // DELETE SUBJECT
        // Principal Only
        // =========================================

        [Authorize(Roles = "Principal")]
        [HttpDelete("subject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
                return NotFound("Subject not found");

            _context.Subjects.Remove(subject);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Subject {id} Deleted Successfully"
            });
        }
    }
}