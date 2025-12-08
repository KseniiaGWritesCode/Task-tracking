using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ManagerId { get; set; }
        public Coworker Manager { get; set; }
        public int EmployeeId { get; set; }
        public Coworker Employee { get; set; }
    }
}
