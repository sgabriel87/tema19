using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tema19.Data;
using tema19.DTOs;
using tema19.Models;

namespace tema19.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public AddressController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<AddressDTO>> GetAddress(int studentId)
        {
            var student = await _context.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null || student.Address == null)
            {
                return NotFound();
            }

            var address = student.Address;

            return new AddressDTO
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street,
                Nr = address.Nr
            };
        }

        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateAddress(int studentId, AddressDTO addressDTO)
        {
            var student = await _context.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }

            if (student.Address == null)
            {
                var newAddress = new Address
                {
                    City = addressDTO.City,
                    Street = addressDTO.Street,
                    Nr = addressDTO.Nr
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                student.AddressId = newAddress.Id;
            }
            else
            {
                var address = student.Address;
                address.City = addressDTO.City;
                address.Street = addressDTO.Street;
                address.Nr = addressDTO.Nr;

                _context.Entry(address).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteAddress(int studentId)
        {
            var student = await _context.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null || student.Address == null)
            {
                return NotFound();
            }

            var address = student.Address;
            student.AddressId = null;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}