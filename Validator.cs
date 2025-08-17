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
        public Category CategoryValidator() 
        {
            Category category = new Category();
            while (true)
            {
                string categoryUserInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(categoryUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<Category>(categoryUserInput.Trim().ToLower(), out category))
                {
                    AnsiConsole.MarkupLine("[magenta1]Category doesn't exhist![/]");
                    continue;
                }

                break;
            }
            return category;
        }

        public ChooseOperation OperationValidator ()
        {
            ChooseOperation chooseOperation = new ChooseOperation();
            AnsiConsole.MarkupLine("[lightcyan1]What do you want to do - create, read, update or delete?[/]");
            while (true)
            {
                string taskUserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(taskUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<ChooseOperation>(taskUserInput.Trim().ToLower(), out chooseOperation))
                {
                    AnsiConsole.MarkupLine("[magenta1]Operation doesn't exhist![/]");
                    continue;
                }
                break;
            }
            return chooseOperation;
        }

        public bool TaskValidator<T> (ProcessingTask processing)
        {
            string taskName;
            DateTime taskDueDate;
            string taskDescription;
            Priority taskPriority;
            Project taskProject;
            Coworker taskManager;
            Coworker taskEmployee;

            try
            {
                if (!string.IsNullOrWhiteSpace(processing.Name.Trim('\'')))
                {
                    taskName = processing.Name;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Task name is empty![/]");
                }

                if (DateTime.TryParseExact(processing.DueDate.Trim('\''), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate) ||
                    !string.IsNullOrWhiteSpace(processing.DueDate))
                {
                    taskDueDate = dueDate;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong fortmat of the due date, or it is empty![/]");
                }

                if (!string.IsNullOrWhiteSpace(processing.Description.Trim('\'')))
                {
                    taskDescription = processing.Description;
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

                if (!string.IsNullOrWhiteSpace(processing.Priority))
                {
                    if (Enum.TryParse<Priority>(processing.Priority.Trim('\''), ignoreCase: true, out var priority))
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

                if (!string.IsNullOrWhiteSpace(processing.Project.Trim('\'')))
                {
                    taskProject = KeeperOfData.Projects.First(p => p.Name.Equals(processing.Project.Trim('\''), StringComparison.OrdinalIgnoreCase));
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No project is given![/]");
                }

                if (!string.IsNullOrWhiteSpace(processing.Manager.Trim('\'')))
                {
                    taskManager = KeeperOfData.Coworkers.First(p => p.Name.Equals(processing.Manager.Trim('\''), StringComparison.OrdinalIgnoreCase));
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

                if (!string.IsNullOrWhiteSpace(processing.Employee.Trim('\'')))
                {
                    taskEmployee = KeeperOfData.Coworkers.First(p => p.Name.Equals(processing.Employee.Trim('\''), StringComparison.OrdinalIgnoreCase));
                    if (taskEmployee == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                    }
                }
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong! Please check your input thoroughly and try again.[/]");
            }


            return true;
        }
    }
}
