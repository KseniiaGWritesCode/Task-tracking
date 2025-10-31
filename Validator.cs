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
using Npgsql;
using BCrypt.Net;

namespace TaskTracking
{
    public static class Validator
    {
        private static readonly CoworkerRepo coworkerRepo;
        public static Coworker ValidateLogin(string login)
        {
            Commands command = new Commands();
            if(login != null)
            {
                string[] checkLogin = login.Split(' ');
                if (checkLogin.Length == 2)
                {
                    var existingUser = coworkerRepo.CheckIfUserExists(checkLogin[0].Trim());
                    if (existingUser == true)
                    {
                        var passwordHash = coworkerRepo.GetPasswordHash(checkLogin[0].Trim());
                        if (BCrypt.Net.BCrypt.Verify(checkLogin[1].Trim(), passwordHash))
                        {
                            var coworker = coworkerRepo.GetCoworkerByMail(checkLogin[0].Trim(), checkLogin[1].Trim());
                            return coworker;
                        }
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
            }
            return null;
        }

        public static Commands? ValidateCommand(string userInput)
        {
            Commands command = new Commands();

            if (userInput != null)
            {
                if (Enum.TryParse<Commands>(userInput, ignoreCase: true, out command))
                {
                    return command;
                }
                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Operation doesn't exhist![/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
            }
            return null;
        }
        public static void ValidateRequest()
        {

        }
        public static Category ValidateCategory() 
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

                if (!Enum.TryParse<Category>(categoryUserInput.Trim(), ignoreCase: true, out category))
                {
                    AnsiConsole.MarkupLine("[magenta1]Category doesn't exhist![/]");
                    continue;
                }

                break;
            }
            return category;
        }

        public static ChooseFilter FilterValidator()
        {
            ChooseFilter chooseFilter = new ChooseFilter();
            while (true)
            {
                string userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<ChooseFilter>(userInput.Trim(), ignoreCase: true, out chooseFilter))
                {
                    AnsiConsole.MarkupLine("[magenta1]Filter doesn't exhist![/]");
                    continue;
                }
                break;
            }
            return chooseFilter;
        }

        public static bool TaskValidator<T> (ProcessingTask processing)
        {
            bool task = true;
            string taskName;
            string taskDescription;
            Priority taskPriority;
            Project taskProject;
            Coworker taskManager;
            Coworker taskEmployee;

            try
            {
                if (!string.IsNullOrWhiteSpace(processing.Name.Trim('\'')))
                {
                    taskName = processing.Name.Trim('\'');
                    processing.Name = taskName;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Task name is empty![/]");
                    task = false;
                }

                if (DateTime.TryParseExact(processing.DueDate.Trim('\''), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate) &&
                    !string.IsNullOrWhiteSpace(processing.DueDate))
                {
                    processing.DueDateFinal = dueDate;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong fortmat of the due date, or it is empty![/]");
                    task = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Description.Trim('\'')))
                {
                    taskDescription = processing.Description.Trim('\'');
                    int countChars = 0;
                    foreach (char c in taskDescription)
                    {
                        countChars++;
                    }
                    if (countChars < 10)
                    {
                        AnsiConsole.MarkupLine("[magenta1]The description should be at least 10 characters long, you lazy jerk![/]");
                        task = false;
                    }

                    processing.Description = taskDescription;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Description is empty![/]");
                    task = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Priority))
                {
                    if (Enum.TryParse<Priority>(processing.Priority.Trim('\''), ignoreCase: true, out var priority))
                        processing.PriorityFinal = priority;

                    else
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong priority![/]");
                        task = false;
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No priority is given![/]");
                    task = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Project.Trim('\'')))
                {
                    processing.ProjectFinal = KeeperOfData.Projects.FirstOrDefault(p => p.Name.Equals(processing.Project.Trim('\''), StringComparison.OrdinalIgnoreCase));
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No project is given![/]");
                    task = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Manager.Trim('\'')))
                {
                    taskManager = KeeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(processing.Manager.Trim('\''), StringComparison.OrdinalIgnoreCase));
                    if (taskManager == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                        task = false;
                    }
                    if (taskManager.Position != Position.Manager)
                    {
                        AnsiConsole.MarkupLine("[magenta1]This employee can't be manager yet![/]");
                        task = false;
                    }
                    processing.ManagerFinal = taskManager;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No manager is given![/]");
                    task = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Employee.Trim('\'')))
                {
                    taskEmployee = KeeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(processing.Employee.Trim('\''), StringComparison.OrdinalIgnoreCase));
                    if (taskEmployee == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                        task = false;
                    }
                    processing.EmployeeFinal = taskEmployee;
                }
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong! Please check your input thoroughly and try again.[/]");
                task = false;
            }

            return task;
        }

        public static bool ProjectValidator<T>(ProcessingProject processing)
        {
            bool project = true;
            string projectName;
            string projectDescription;
            Priority projectPriority;
            Coworker projectManager;

            try
            {
                if (!string.IsNullOrWhiteSpace(processing.Name.Trim('\'')))
                {
                    projectName = processing.Name.Trim('\'');
                    processing.Name = projectName;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Project name is empty![/]");
                    project = false;
                }

                if (DateTime.TryParseExact(processing.DueDate.Trim('\''), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate) ||
                    !string.IsNullOrWhiteSpace(processing.DueDate))
                {
                    processing.DueDateFinal = dueDate;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong fortmat of the due date, or it is empty![/]");
                    project = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Description.Trim('\'')))
                {
                    projectDescription = processing.Description.Trim('\'');
                    int countChars = 0;
                    foreach (char c in projectDescription)
                    {
                        countChars++;
                    }
                    if (countChars < 10)
                    {
                        AnsiConsole.MarkupLine("[magenta1]The description should be at least 10 characters long, you lazy jerk![/]");
                        project = false;
                    }

                    processing.Description = projectDescription;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Description is empty![/]");
                    project = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Priority))
                {
                    if (Enum.TryParse<Priority>(processing.Priority.Trim('\''), ignoreCase: true, out var priority))
                        processing.PriorityFinal = priority;

                    else
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong priority![/]");
                        project = false;
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No priority is given![/]");
                    project = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Manager.Trim('\'')))
                {
                    projectManager = KeeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(processing.Manager.Trim('\''), StringComparison.OrdinalIgnoreCase));
                    if (projectManager == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Employee doesn't exhist![/]");
                        project = false;
                    }
                    if (projectManager.Position != Position.Manager)
                    {
                        AnsiConsole.MarkupLine("[magenta1]This employee can't be manager yet![/]");
                        project = false;
                    }
                    processing.ManagerFinal = projectManager;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No manager is given![/]");
                    project = false;
                }
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong! Please check your input thoroughly and try again.[/]");
                project = false;
            }

            return project;
        }

        public static bool CoworkerValidator<T>(ProcessingCoworker processing)
        {
            bool coworker = true;
            string coworkerName;
            string coworkerBirthday;
            string coworkerEmail;
            Position coworkerPosition;

            try
            {
                if (!string.IsNullOrWhiteSpace(processing.Name.Trim('\'')))
                {
                    coworkerName = processing.Name.Trim('\'');
                    processing.Name = coworkerName;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Coworker name is empty![/]");
                    coworker = false;
                }

                if (DateTime.TryParseExact(processing.Birthday.Trim('\''), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthday) ||
                    !string.IsNullOrWhiteSpace(processing.Birthday))
                {
                    processing.BirthdayFilnal = birthday;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong fortmat of birthday, or it is empty![/]");
                    coworker = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.EMail.Trim('\'')))
                {
                    coworkerEmail = processing.EMail.Trim('\'');
                    int countChars = 0;
                    foreach (char c in coworkerEmail)
                    {
                        countChars++;
                    }
                    if (countChars < 5)
                    {
                        AnsiConsole.MarkupLine("[magenta1]The e-mail is too short![/]");
                        coworker = false;
                    }

                    processing.EMail = coworkerEmail;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]E-mail is empty![/]");
                    coworker = false;
                }

                if (!string.IsNullOrWhiteSpace(processing.Position))
                {
                    if (Enum.TryParse<Position>(processing.Position.Trim('\''), ignoreCase: true, out var position))
                        processing.PositionFinal = position;

                    else
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong position![/]");
                        coworker = false;
                    }
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No position is given![/]");
                    coworker = false;
                }
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong! Please check your input thoroughly and try again.[/]");
                coworker = false;
            }

            return coworker;
        }

        public static bool ProjectNameValidator(string projectName)
        {
            bool valid = true;
            if (!string.IsNullOrWhiteSpace(projectName.Trim()))
            {
                var validProject = KeeperOfData.Projects.FirstOrDefault(p => p.Name.Equals(projectName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (validProject == null)
                {
                    AnsiConsole.MarkupLine("[magenta1]Project doesn't exhist![/]");
                    valid = false;
                }
            }

            else
            {
                AnsiConsole.MarkupLine("[magenta1]No project is given![/]");
                valid = false;
            }
            return valid;
        }

        public static Priority PriorityValidator(string priority)
        {
            Priority validPriority = new Priority();
            if (!string.IsNullOrWhiteSpace(priority.Trim()))
            {
                if (!Enum.TryParse<Priority>(priority.Trim(), ignoreCase: true, out validPriority))
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong priority![/]");
                }
            }

            else
            {
                AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
            }
            return validPriority;
        }

        public static Position PositionValidator(string position)
        {
            Position validPosition = new Position();
            if (!string.IsNullOrWhiteSpace(position.Trim()))
            {
                if (!Enum.TryParse<Position>(position.Trim(), ignoreCase: true, out validPosition))
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong position![/]");
                }
            }

            else
            {
                AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
            }
            return validPosition;
        }

        public static bool CoworkerNameValidator(string coworkerName)
        {
            bool valid = true;
            if (!string.IsNullOrWhiteSpace(coworkerName.Trim()))
            {
                var validCoworker = KeeperOfData.Coworkers.FirstOrDefault(p => p.Name.Equals(coworkerName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (validCoworker == null)
                {
                    AnsiConsole.MarkupLine("[magenta1]Coworker doesn't exhist![/]");
                    valid = false;
                }
            }

            else
            {
                AnsiConsole.MarkupLine("[magenta1]No coworker name is given![/]");
                valid = false;
            }
            return valid;
        }

        public static bool TaskNameValidator(string taskName)
        {
            bool valid = true;
            if (!string.IsNullOrWhiteSpace(taskName.Trim()))
            {
                if (KeeperOfData.Tasks.Any(t => t.Name == taskName.Trim()))
                {
                    valid = true;
                }

                else
                {
                    AnsiConsole.MarkupLine("[magenta1]No task with such name![/]");
                    valid = false;
                }
            }

            else
            {
                AnsiConsole.MarkupLine("[magenta1]Task name is empty![/]");
                valid = false;
            }
            return valid;
        }


        public static void HFGh()
        {
            Func<string, string, (bool success, string error)> func = (arg1, arg2) =>
            {
                return (true, "error");
            };

            ValidateFunc((arg1, arg2) =>
            {



                return (true, "error");
            }
            );
        }

        public static bool ValidateFunc(Func<string, string, (bool success, string error)> func)
        {
            // take input from console
            // invoke func
            // notify user
            // return 

            var result = func.Invoke("111", "fdsf");

            return result.success;
        }
    }
}
