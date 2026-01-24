using System.ComponentModel.DataAnnotations;
using TaskTracking.Entities.Coworker;
using TaskTracking.Entities.Project;

namespace TaskTracking.Entities.Task
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int ProjectId { get; set; }
        public ProjectModel Project { get; set; }
        public int ManagerId { get; set; }
        public CoworkerModel Manager { get; set; }
        public int EmployeeId { get; set; }
        public CoworkerModel Employee { get; set; }
    }
}
