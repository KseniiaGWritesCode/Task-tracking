using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Project : IToDoProperties
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        Priority Priority { get; set; }
        TaskItem Task { get; set; }
        Coworker Manager { get; set; }

        public Project(string name, DateTime dueDate, string description, Priority priority, KeeperOfData tasks, KeeperOfData manager)
            
        { 
            Name = name;
            DueDate = dueDate;
            Description = description;
            Priority = priority;
        }
    }
}
