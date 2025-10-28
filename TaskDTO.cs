using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class TaskDTO
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public Project Project { get; set; }
        public Coworker Manager { get; set; }
        public Coworker Employee { get; set; }
    }
}
