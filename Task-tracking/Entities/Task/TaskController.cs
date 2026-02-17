using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskTracking.Entities.Project;

namespace TaskTracking.Entities.Task
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly AppDbContext _context;

        public TaskController(ILogger<TaskController> logger, AppDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TaskGetDto>>> GetAllTasks()
        {
            List<TaskGetDto> taskDtos = new List<TaskGetDto>();

            try
            {
                var tasks = await _context.Tasks.ToListAsync();
                tasks.ForEach(t =>
                {
                    taskDtos.Add(new TaskGetDto()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        DueDate = t.DueDate,
                        Description = t.Description,
                        Priority = t.Priority,
                        ProjectId = t.ProjectId,
                        ManagerId = t.ManagerId,
                        EmployeeId = t.EmployeeId
                    });
                });
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetAllTasks));
            }

            if (!taskDtos.Any())
            {
                return NotFound();
            }

            return Ok(taskDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            TaskGetDto taskDto = null;

            try
            {
                var task = await _context.Tasks.FindAsync(id);

                if (task == null) { return NotFound(new { message = "Task not found" }); }

                taskDto = new TaskGetDto()
                {
                    Id = task.Id,
                    Name = task.Name,
                    DueDate = task.DueDate,
                    Description = task.Description,
                    Priority = task.Priority,
                    ManagerId = task.ManagerId
                };
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(GetTaskById));
            }

            return Ok(taskDto);
        }




        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
