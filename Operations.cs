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
        List<T> list = new();
        public Operations(List<T> list) 
        { 
           this.list = list;
        }
        public List<T> ShowList (T item)
        {
            return list;
        }

        public List<string> TaskCreate()
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

                FormingNewTask<ProcessingTask>(tasks);
            }
            
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
            }
            return tasks;
        }
        //'Figma prototype' '20.09.2025' 'Clickable prototype for the project' 'low' 'Inhabited Mind' 'John Smith' 'Sarah Jessica Parker'
        public ProcessingTask FormingNewTask<T> (List<string> newItem)
        {
                ProcessingTask processingTask = new ProcessingTask(newItem);

                for (int i = 0; i < newItem.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            processingTask.Name = newItem[i];
                            break;
                        case 1:
                            processingTask.DueDate = newItem[i];
                            break;
                        case 2:
                            processingTask.Description = newItem[i];
                            break;
                        case 3:
                            processingTask.Priority = newItem[i];
                            break;
                        case 4:
                            processingTask.Project = newItem[i];
                            break;
                        case 5:
                            processingTask.Manager = newItem[i];
                            break;
                        case 6:
                            processingTask.Employee = newItem[i];
                            break;
                    }
                }
            return processingTask;
        }
    }
}
