using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class TaskItem : IToDoProperties
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public Project Project { get; set; }
        public Coworker Manager { get; set; }
        public Coworker Employee { get; set; }

        public TaskItem (string name, DateTime dueDate, string description, Priority priority, Project project, Coworker manager, Coworker employee) 
        { 
            Name = name;
            DueDate = dueDate;
            Description = description;
            Priority = priority;
            Project = project;
            Employee = employee;
            Manager = manager;
        }
    }
}
