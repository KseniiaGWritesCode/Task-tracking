using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Validator
    {
        public bool CategoryValidator() 
        {
            
            while (true)
            {
                string categoryUserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(categoryUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<Category>(categoryUserInput.Trim().ToLower(), out var category))
                {
                    AnsiConsole.MarkupLine("[magenta1]Category doesn't exhist![/]");
                    continue;
                }
                break;
            }
            return true;
        }

        public bool OperationValidator<T> (Operations<T> ops)
        {
            AnsiConsole.MarkupLine("[lightcyan1]What do you want to do - create, read, update or delete?[/]");
            while (true)
            {
                string taskUserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(taskUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<ChooseOperation>(taskUserInput.Trim().ToLower(), out var operation))
                {
                    AnsiConsole.MarkupLine("[magenta1]Operation doesn't exhist![/]");
                    continue;
                }
            }
        }

        public bool TaskValidator<T> (Operations<T> ops)
        {
            ProcessingTask processingTask = new ProcessingTask();
            KeeperOfData keeperOfData = new KeeperOfData();
            string taskName;
            DateTime taskDueDate;
            string taskDescription;
            Priority taskPriority;
            Project taskProject;
            Coworker taskManager;
            Coworker taskEmployee;

            try
            {
                if (!string.IsNullOrWhiteSpace(processingTask.Name))
                {
                    taskName = processingTask.Name;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Task name is empty![/]");
                }

                if (DateTime.TryParseExact(processingTask.DueDate.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate) ||
                    !string.IsNullOrWhiteSpace(processingTask.DueDate))
                {
                    taskDueDate = dueDate;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong fortmat of the due date, or it is empty![/]");
                }

                if (!string.IsNullOrWhiteSpace(processingTask.Description))
                {
                    taskDescription = processingTask.Description;
                    int countChars = 0;
                    foreach (char c in taskDescription)
                    {
                        countChars++;
                    }
                    if (countChars < 10)
                    {
                        AnsiConsole.MarkupLine("[magenta1]The description should be at least 10 characters long, you lazy jerk![/]");
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Description is empty![/]");
                }

                if (!string.IsNullOrWhiteSpace(processingTask.Priority))
                {
                    if (Enum.TryParse<Priority>(processingTask.Priority, out var priority))
                        taskPriority = priority;

                    else
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong priority![/]");
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No priority is given![/]");
                }

                if (!string.IsNullOrWhiteSpace(processingTask.Project))
                {
                    taskProject = keeperOfData.Projects.FirstOrDefault(p => p.Name.Equals(processingTask.Project, StringComparison.OrdinalIgnoreCase));
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No project is given![/]");
                }

                if (!string.IsNullOrWhiteSpace(processingTask.Manager))
                {
                    taskManager = keeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(processingTask.Manager, StringComparison.OrdinalIgnoreCase));
                    if (taskManager == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                    }
                    if (taskManager.Position != Position.Manager)
                    {
                        AnsiConsole.MarkupLine("[magenta1]This employee can't be manager yet![/]");
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No manager is given![/]");
                }

                if (!string.IsNullOrWhiteSpace(processingTask.Employee))
                {
                    taskEmployee = keeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(processingTask.Employee, StringComparison.OrdinalIgnoreCase));
                    if (taskEmployee == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                    }
                }
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong! Please check your inpur thoroughly and try again.[/]");
            }

            return true;
        }
    }
}
