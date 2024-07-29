using Microsoft.AspNetCore.Mvc;
using tema19.Data;
using tema19.Models;

namespace tema19.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public SeedController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SeedDatabase()
        {
            if (_context.Students.Any() || _context.Addresses.Any())
            {
                return BadRequest("Database already seeded.");
            }

            var addresses = new[]
            {
                new Address { City = "Oradea", Street = "Centru", Nr = "101" },
                new Address { City = "Cluj", Street = "Centru", Nr = "202" },
                new Address { City = "Timisoara", Street = "Centru", Nr = "303" }
            };

            var students = new[]
            {
                new Student { FirstName = "Thomas", LastName = "Mate", Age = 20, Address = addresses[0] },
                new Student { FirstName = "Dia", LastName = "Toth", Age = 22, Address = addresses[1] },
                new Student { FirstName = "Anna", LastName = "Tudor", Age = 21, Address = addresses[2] }
            };

            _context.Addresses.AddRange(addresses);
            _context.Students.AddRange(students);

            await _context.SaveChangesAsync();

            return Ok("Database seeded successfully.");
        }
    }
}
