using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

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




        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
