using System.ComponentModel.DataAnnotations;
using TaskTracking.Entities.Coworker;

namespace TaskTracking.Entities.Project
{
    public class ProjectModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int ManagerId { get; set; }
        public CoworkerModel Manager { get; set; }
    }
}
