using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ProcessingProject
    {
        public string Name { get; set; }
        public string DueDate { get; set; }
        public DateTime DueDateFinal { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public Priority PriorityFinal { get; set; }
        public string Manager { get; set; }
        public Coworker ManagerFinal { get; set; }

        public ProcessingProject(List<string> input) 
        {
            if (input.Count >= 5)
            {
                Name = input[0];
                DueDate = input[1];
                Description = input[2];
                Priority = input[3];
                Manager = input[4];
            }
        }

        public Project TransferToProjectItem(string name, DateTime dueDate, string description, Priority priority, Coworker manager)
        {
            Name = name;
            DueDateFinal = dueDate;
            Description = description;
            PriorityFinal = priority;
            ManagerFinal = manager;

            Project projectItem = new(Name, DueDateFinal, Description, PriorityFinal, ManagerFinal);
            return projectItem;
        }
    }
}

//'Personal website' '01.12.2025' 'Create a personal website, where all my services and creations are present' 'medium' 'Jane Doe'