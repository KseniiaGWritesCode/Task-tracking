using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Operations<T>
    {
        KeeperOfData listOfTasks = new KeeperOfData();
        KeeperOfData listOfProjects = new KeeperOfData();
        KeeperOfData listOfCoworkers = new KeeperOfData();

        List<T> list = new();
        public Operations(List<T> list) 
        { 
           
        }
        public List<T> See(T item)
        {
            return list;
        }

        public void Create<T> (T item)
        {
            switch(item)
            {
                case Category.tasks:
                    TaskCreate();
                    break;
            }
        }

        public void TaskCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the new task, each supplemented with '':[/]");
            AnsiConsole.MarkupLine("[darkolivegreen1] 'name' 'dd.mm.yyyy (due date)' 'description' 'priority (low, medium or high)' 'project' 'manager' 'employee'[/]");
            
            string taskInput = Console.ReadLine();
            List<string> tasks = new List<string>();

            try
            {
                var matches = Regex.Matches(taskInput, "'([^']*)'|(\\S+)");
                foreach (Match match in matches)
                {
                    tasks.Add(match.Value);
                }

                FormingNewTask(tasks);
            }
            
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
                return;
            }
        }

        public void FormingNewTask (List<string> newTask)
        {
            ProcessingTask processingTask = new ProcessingTask();

            for (int i = 0; i <= newTask.Count; i++)
            {
                switch(i)
                {
                    case 0:
                        newTask[i] = processingTask.Name;
                        break;
                    case 1:
                        newTask[i] = processingTask.DueDate;
                        break;
                    case 2:
                        newTask[i] = processingTask.Description;
                        break;
                    case 3:
                        newTask[i] = processingTask.Priority;
                        break;
                    case 4:
                        newTask[i] = processingTask.Project;
                        break;
                    case 5:
                        newTask[i] = processingTask.Manager;
                        break;
                    case 6:
                        newTask[i] = processingTask.Employee;
                        break;
                }
            }
        }
    }
}
