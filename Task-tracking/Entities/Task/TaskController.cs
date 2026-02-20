using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TaskTracking.Entities.Task;
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


        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskById(int id)
        {
            TaskModel task = null;
            try
            {
                task = await _context.Tasks.FindAsync(id);

                if (task == null) { return NotFound(new { message = "Task not found" }); }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(DeleteTaskById));
            }

            return Ok(new { message = "Creature deleted successfully", task });
        }

        //[Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto newTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TaskModel taskModel = new TaskModel()
            {
                Name = newTask.Name,
                DueDate = newTask.DueDate.Value,
                Description = newTask.Description,
                Priority = newTask.Priority.Value,
                ProjectId = newTask.ProjectId.Value,
                ManagerId = newTask.ManagerId.Value,
                EmployeeId = newTask.EmployeeId.Value
            };

            try
            {
                var result = await _context.Tasks.AddAsync(taskModel);
                taskModel = result.Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(CreateTask));
            }

            TaskGetDto taskGetDto = null;
            if (taskModel != null)
            {
                taskGetDto = new TaskGetDto();
                taskGetDto.Id = taskModel.Id;
                taskGetDto.Name = taskModel.Name;
                taskGetDto.DueDate = taskModel.DueDate;
                taskGetDto.Description = taskModel.Description;
                taskGetDto.Priority = taskModel.Priority;
                taskGetDto.ProjectId = taskModel.ProjectId;
                taskGetDto.ManagerId = taskModel.ManagerId;
                taskGetDto.EmployeeId = taskModel.EmployeeId;
            }

            return CreatedAtAction(nameof(CreateTask), taskGetDto);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto updatedTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null) { return NotFound(new { message = "Task not found" }); }

            task.Name = updatedTask.Name;
            task.DueDate = updatedTask.DueDate.Value;
            task.Description = updatedTask.Description;
            task.Priority = updatedTask.Priority.Value;
            task.ProjectId = updatedTask.ProjectId.Value;
            task.ManagerId = updatedTask.ManagerId.Value;
            task.EmployeeId = updatedTask.EmployeeId.Value;

            try
            {
                task = _context.Tasks.Update(task).Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ReturnSystemErrorWithLog(ex, nameof(UpdateTask));
            }

            TaskGetDto taskGetDto = null;
            if (task != null)
            {
                taskGetDto = new TaskGetDto();
                taskGetDto.Id = task.Id;
                taskGetDto.Name = task.Name;
                taskGetDto.DueDate = task.DueDate;
                taskGetDto.Description = task.Description;
                taskGetDto.Priority = task.Priority;
                taskGetDto.ProjectId = task.ProjectId;
                taskGetDto.ManagerId = task.ManagerId;
                taskGetDto.EmployeeId = task.EmployeeId;
            }

            return Ok(new { message = "Task updated successfully", taskGetDto });
        }


        private ObjectResult ReturnSystemErrorWithLog(Exception ex, string methodName)
        {
            _logger.LogError(ex, $"On {methodName}");
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return StatusCode(500, $"Internal server error: {message}");
        }
    }
}
