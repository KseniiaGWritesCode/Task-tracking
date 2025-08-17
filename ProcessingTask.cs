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

        public TaskItem TransferToTaskItem (string name, DateTime dueDate, string description, Priority priority, Project project, Coworker manager, Coworker employee)
        {
            Name = name;
            //DateTime.TryParse(DueDate, out dueDate);
            DueDateFinal = dueDate;
            Description = description;
            //Enum.TryParse(Priority, out priority);
            PriorityFinal = priority;
            //project = KeeperOfData.Projects.First(p => p.Name == Name);
            ProjectFinal = project;
            //manager = KeeperOfData.Coworkers.First(p => p.Name == Manager);
            ManagerFinal = manager;
            //employee = KeeperOfData.Coworkers.First (p => p.Name == Employee);
            EmployeeFinal = employee;

            TaskItem taskItem = new(Name, DueDateFinal, Description, PriorityFinal, ProjectFinal, ManagerFinal, EmployeeFinal);
            return taskItem;
        }
    }
}
