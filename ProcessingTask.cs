using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ProcessingTask
    {
        public string Name { get; set; }
        public string DueDate { get; set; }
        public DateTime DueDateFinal { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public Priority PriorityFinal { get; set; }
        public string Project { get; set; }
        public Project ProjectFinal { get; set; }
        public string Manager { get; set; }
        public Coworker ManagerFinal { get; set; }
        public string Employee { get; set; }
        public Coworker EmployeeFinal { get; set; }

        public ProcessingTask(List<string> input)
        {
            if (input.Count >= 7)
            {
                Name = input[0];
                DueDate = input[1];
                Description = input[2];
                Priority = input[3];
                Project = input[4];
                Manager = input[5];
                Employee = input[6];
            }
        }

        public Task TransferToTaskItem (string name, DateTime dueDate, string description, Priority priority, Project project, Coworker manager, Coworker employee)
        {
            Name = name;
            DueDateFinal = dueDate;
            Description = description;
            PriorityFinal = priority;
            ProjectFinal = project;
            ManagerFinal = manager;
            EmployeeFinal = employee;

            Task taskItem = new(Name, DueDateFinal, Description, PriorityFinal, ProjectFinal, ManagerFinal, EmployeeFinal);
            return taskItem;
        }
    }
}
//'User Flow for Inhabited Mind' '15.09.2025' 'Make user flow' 'high' 'Inhabited Mind' 'John Smith' 'Mary Cole'