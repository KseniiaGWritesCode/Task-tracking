using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Xml.Linq;
using TaskTracking.Entities.Project;

namespace TaskTracking.Entities.Coworker
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoworkerController : ControllerBase
    {
        private readonly ILogger<CoworkerController> _logger;
        private readonly AppDbContext _context;

        public CoworkerController(ILogger<CoworkerController> logger, AppDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CoworkerGetAllDto>>> GetAllCoworkers()
        {
            List<CoworkerGetAllDto> coworkersDtos = new List<CoworkerGetAllDto>();

            try
            {
                var coworkers = await _context.Coworkers.ToListAsync();
                coworkers.ForEach(c =>
                {
                    coworkersDtos.Add(new CoworkerGetAllDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        EMail = c.EMail
                    });
                });
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetAllCoworkers));
            }

            if (!coworkersDtos.Any())
            {
                return NotFound();
            }

            return Ok(coworkersDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoworkerById(int id)
        {
            CoworkerGetDto coworkerDto = null;

            try
            {
                var coworker = await _context.Coworkers.FindAsync(id);

                if (coworker == null) { return NotFound(new { message = "Coworker not found" }); }

                coworkerDto = new CoworkerGetDto()
                {
                    Id = id,
                    Name = coworker.Name,
                    Birthday = coworker.Birthday,
                    EMail = coworker.EMail,
                    Position = coworker.Position
                };
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetCoworkerById));
            }

            return Ok(coworkerDto);
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoworkerById(int id)
        {
            CoworkerModel coworker = null;
            try
            {
                coworker = await _context.Coworkers.FindAsync(id);

                if (coworker == null) { return NotFound(new { message = "Coworker not found" }); }

                _context.Coworkers.Remove(coworker);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(DeleteCoworkerById));
            }

            return Ok(new { message = "Coworker deleted successfully", coworker });
        }

        //[Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCoworker([FromBody] CoworkerCreateDto newCoworker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Coworkers.AnyAsync(c => c.EMail == newCoworker.EMail))
            {
                return BadRequest($"Coworker with EMail {newCoworker.EMail} already exists");
            }

            CoworkerModel coworkerModel = new CoworkerModel()
            {
                Name = newCoworker.Name,
                Birthday = newCoworker.Birthday.Value,
                EMail = newCoworker.EMail,
                Position = newCoworker.Position.Value,
                Password = BCrypt.Net.BCrypt.HashPassword(newCoworker.Password),
                FavoriteToy = newCoworker.FavoriteToy
            };

            try
            {
                var result = await _context.Coworkers.AddAsync(coworkerModel);
                coworkerModel = result.Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(CreateCoworker));
            }

            CoworkerGetDto coworkerGetDto = null;
            //coworkerModel = await _context.Coworkers.FirstOrDefaultAsync(c => c.EMail == newCoworker.EMail);
            if (coworkerModel != null)
            {
                coworkerGetDto = new CoworkerGetDto();
                coworkerGetDto.Id = coworkerModel.Id;
                coworkerGetDto.Name = coworkerModel.Name;
                coworkerGetDto.Birthday = coworkerModel.Birthday;
                coworkerGetDto.EMail = coworkerModel.EMail;
                coworkerGetDto.Position = coworkerModel.Position;
            }

            return CreatedAtAction(nameof(CreateCoworker), coworkerGetDto);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoworker(int id, [FromBody] CoworkerUpdateDto updatedCoworker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var coworker = await _context.Coworkers.FindAsync(id);

            if (coworker == null) { return NotFound(new { message = "Coworker not found" }); }

            coworker.Name = updatedCoworker.Name;
            coworker.Birthday = updatedCoworker.Birthday.Value;
            coworker.Position = updatedCoworker.Position.Value;
            coworker.FavoriteToy = updatedCoworker.FavoriteToy;

            try
            {
                coworker = _context.Coworkers.Update(coworker).Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(UpdateCoworker));
            }

            CoworkerGetDto coworkerGetDto = null;
            //coworker = await _context.Coworkers.FindAsync(id);
            if (coworker != null)
            {
                coworkerGetDto = new CoworkerGetDto();
                coworkerGetDto.Id = coworker.Id;
                coworkerGetDto.Name = coworker.Name;
                coworkerGetDto.Birthday = coworker.Birthday;
                coworkerGetDto.EMail = coworker.EMail;
                coworkerGetDto.Position = coworker.Position;
            }

            return Ok(new { message = "Coworker updated successfully", coworkerGetDto });
        }

        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
