using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Task : IToDoProperties
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        Priority Priority { get; set; }
        Project Project { get; set; }
        Coworker Manager { get; set; }
        Coworker Employee { get; set; }

        public Task(string name, DateTime dueDate, string description, Priority priority, Project project, Coworker manager, Coworker employee) 
        { 
            Name = name;
            DueDate = dueDate;
            Description = description;
            Priority = priority;
            Project = project;
            Manager = manager;
            Employee = employee;
        }
    }
}
