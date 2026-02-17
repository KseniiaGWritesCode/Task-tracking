using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskTracking.Entities.Project;

namespace TaskTracking.Entities.Project
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly AppDbContext _context;

        public ProjectController(ILogger<ProjectController> logger, AppDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProjectGetDto>>> GetAllProjects()
        {
            List<ProjectGetDto> projectDtos = new List<ProjectGetDto>();

            try
            {
                var projects = await _context.Projects.ToListAsync();
                projects.ForEach(p =>
                {
                    projectDtos.Add(new ProjectGetDto()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        DueDate = p.DueDate,
                        Description = p.Description,
                        Priority = p.Priority,
                        ManagerId = p.ManagerId
                    });
                });
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetAllProjects));
            }

            if (!projectDtos.Any())
            {
                return NotFound();
            }

            return Ok(projectDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            ProjectGetDto projectDto = null;

            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null) { return NotFound(new { message = "Project not found" }); }

                projectDto = new ProjectGetDto()
                {
                    Id = project.Id,
                    Name = project.Name,
                    DueDate = project.DueDate,
                    Description = project.Description,
                    Priority = project.Priority,
                    ManagerId = project.ManagerId
                };
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetProjectById));
            }

            return Ok(projectDto);
        }




        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
