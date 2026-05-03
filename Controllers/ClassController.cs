using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Api.Data;
using SchoolManagementSystem.Api.DTOs;
using SchoolManagementSystem.Api.Models;

namespace SchoolManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("sms")]
    public class ClassController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        // GET: sms/classes
        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await _context.Classes
                .Include(c => c.ClassTeacher)
                .Include(c => c.Students).ThenInclude(s => s.Student)
                .Include(c => c.AssignedTeachers).ThenInclude(t => t.Teacher)
                .Include(c => c.Subjects).ThenInclude(s => s.Subject)
                .ToListAsync();

            var result = classes.Select(cls => new ClassDto
            {
                Id = cls.Id,
                ClassName = cls.ClassName,

                ClassTeacherName = cls.ClassTeacher != null
        ? cls.ClassTeacher.FirstName + " " + cls.ClassTeacher.LastName
        : null,

                Students = cls.Students.Select(s => new UserDto
                {
                    Id = s.Student.Id,
                    FullName = s.Student.FirstName + " " + s.Student.LastName
                }).ToList(),

                AssignedTeachers = cls.AssignedTeachers.Select(t => new UserDto
                {
                    Id = t.Teacher.Id,
                    FullName = t.Teacher.FirstName + " " + t.Teacher.LastName
                }).ToList(),

                Subjects = cls.Subjects.Select(s =>
                    s.Subject.Name
                ).ToList()
            });

            return Ok(result);
        }

        // GET: sms/class/1
        [HttpGet("class/{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            var cls = await _context.Classes
                .Include(c => c.ClassTeacher)
                .Include(c => c.Students).ThenInclude(s => s.Student)
                .Include(c => c.AssignedTeachers).ThenInclude(t => t.Teacher)
                .Include(c => c.Subjects).ThenInclude(s => s.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cls == null)
                return NotFound("Class not found");

            var result = new ClassDto
            {
                Id = cls.Id,
                ClassName = cls.ClassName,

                ClassTeacherName = cls.ClassTeacher != null
        ? cls.ClassTeacher.FirstName + " " + cls.ClassTeacher.LastName
        : null,

                Students = cls.Students.Select(s => new UserDto
                {
                    Id = s.Student.Id,
                    FullName = s.Student.FirstName + " " + s.Student.LastName
                }).ToList(),

                AssignedTeachers = cls.AssignedTeachers.Select(t => new UserDto
                {
                    Id = t.Teacher.Id,
                    FullName = t.Teacher.FirstName + " " + t.Teacher.LastName
                }).ToList(),


                Subjects = cls.Subjects.Select(s =>
                    s.Subject.Name
                ).ToList()
            };

            return Ok(result);
        }

        // POST: sms/class
        [HttpPost("class")]
        public async Task<IActionResult> AddClass([FromBody] Class cls)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Classes.Add(cls);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Class Added Successfully",
                data = cls
            });
        }

        // PUT: sms/class/1
        [HttpPut("class/{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] Class updatedClass)
        {
            var cls = await _context.Classes.FindAsync(id);

            if (cls == null)
                return NotFound("Class not found");

            cls.ClassName = updatedClass.ClassName;
            cls.ClassTeacherId = updatedClass.ClassTeacherId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Class {id} Updated Successfully"
            });
        }

        // DELETE: sms/class/1
        [HttpDelete("class/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var cls = await _context.Classes.FindAsync(id);

            if (cls == null)
                return NotFound("Class not found");

            _context.Classes.Remove(cls);
            await _context.SaveChangesAsync();

            return Ok($"Class {id} Deleted Successfully");
        }

        // =============================
        // RELATIONSHIP ENDPOINTS
        // =============================

        [HttpPost("class/{classId}/add-student/{studentId}")]
        public async Task<IActionResult> AddStudent(int classId, int studentId)
        {
            var exists = await _context.ClassStudents
                .AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);

            if (exists)
                return BadRequest("Student already assigned");

            _context.ClassStudents.Add(new ClassStudent
            {
                ClassId = classId,
                StudentId = studentId
            });

            await _context.SaveChangesAsync();

            return Ok("Student assigned successfully");
        }

        [HttpPost("class/{classId}/add-teacher/{teacherId}")]
        public async Task<IActionResult> AddTeacher(int classId, int teacherId)
        {
            var exists = await _context.ClassTeachers
                .AnyAsync(ct => ct.ClassId == classId && ct.TeacherId == teacherId);

            if (exists)
                return BadRequest("Teacher already assigned");

            _context.ClassTeachers.Add(new ClassTeacher
            {
                ClassId = classId,
                TeacherId = teacherId
            });

            await _context.SaveChangesAsync();

            return Ok("Teacher assigned successfully");
        }

        [HttpPost("class/{classId}/add-subject/{subjectId}")]
        public async Task<IActionResult> AddSubject(int classId, int subjectId)
        {
            var exists = await _context.ClassSubjects
                .AnyAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId);

            if (exists)
                return BadRequest("Subject already assigned");

            _context.ClassSubjects.Add(new ClassSubject
            {
                ClassId = classId,
                SubjectId = subjectId
            });

            await _context.SaveChangesAsync();

            return Ok("Subject assigned successfully");
        }
    }
}