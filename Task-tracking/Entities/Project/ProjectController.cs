using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskTracking.Entities.Project;
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



        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectById(int id)
        {
            ProjectModel project = null;
            try
            {
                project = await _context.Projects.FindAsync(id);

                if (project == null) { return NotFound(new { message = "Project not found" }); }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(DeleteProjectById));
            }

            return Ok(new { message = "Project deleted successfully", project });
        }

        //[Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto newProject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProjectModel projectModel = new ProjectModel()
            {
                Name = newProject.Name,
                DueDate = newProject.DueDate.Value,
                Description = newProject.Description,
                Priority = newProject.Priority.Value,
                ManagerId = newProject.ManagerId.Value
            };

            try
            {
                var result = await _context.Projects.AddAsync(projectModel);
                projectModel = result.Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(CreateProject));
            }

            ProjectGetDto projectGetDto = null;
            if (projectModel != null)
            {
                projectGetDto = new ProjectGetDto();
                projectGetDto.Id = projectModel.Id;
                projectGetDto.Name = projectModel.Name;
                projectGetDto.DueDate = projectModel.DueDate;
                projectGetDto.Description = projectModel.Description;
                projectGetDto.Priority = projectModel.Priority;
                projectGetDto.ManagerId = projectModel.ManagerId;
            }

            return CreatedAtAction(nameof(CreateProject), projectGetDto);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto updatedProject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _context.Projects.FindAsync(id);

            if (project == null) { return NotFound(new { message = "Project not found" }); }

            project.Name = updatedProject.Name;
            project.DueDate = updatedProject.DueDate.Value;
            project.Description = updatedProject.Description;
            project.Priority = updatedProject.Priority.Value;
            project.ManagerId = updatedProject.ManagerId.Value;

            try
            {
                project = _context.Projects.Update(project).Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(UpdateProject));
            }

            ProjectGetDto projectGetDto = null;
            if (project != null)
            {
                projectGetDto = new ProjectGetDto();
                projectGetDto.Id = project.Id;
                projectGetDto.Name = project.Name;
                projectGetDto.DueDate = project.DueDate;
                projectGetDto.Description = project.Description;
                projectGetDto.Priority = project.Priority;
                projectGetDto.ManagerId = project.ManagerId;
            }

            return Ok(new { message = "Project updated successfully", projectGetDto });
        }

        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
