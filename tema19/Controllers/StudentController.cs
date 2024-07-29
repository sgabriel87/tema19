using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tema19.Data;
using tema19.DTOs;
using tema19.Models;

namespace tema19.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            return await _context.Students
                .Select(s => new StudentDTO
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Age = s.Age,
                    AddressId = s.AddressId
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return new StudentDTO
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                AddressId = student.AddressId
            };
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(CreateStudentDTO createStudentDTO)
        {
            var student = new Student
            {
                FirstName = createStudentDTO.FirstName,
                LastName = createStudentDTO.LastName,
                Age = createStudentDTO.Age
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDTO updateStudentDTO)
        {
            if (id != updateStudentDTO.Id)
            {
                return BadRequest();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"No Student with {id} was found");
            }

            student.FirstName = updateStudentDTO.FirstName;
            student.LastName = updateStudentDTO.LastName;
            student.Age = updateStudentDTO.Age;
            student.AddressId = updateStudentDTO.AddressId;

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id, [FromQuery] bool deleteAddress = false)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            if (deleteAddress && student.AddressId.HasValue)
            {
                var address = await _context.Addresses.FindAsync(student.AddressId);
                if (address != null)
                {
                    _context.Addresses.Remove(address);
                }
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
