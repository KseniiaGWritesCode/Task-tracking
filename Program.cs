using Spectre.Console;
using System.Globalization;

namespace TaskTracking
{
    public class Program
    {
        static void Main(string[] args)
        {
            var table = new Table();
            table.AddColumn("[bold yellow]Command[/]");
            table.AddColumn("[bold yellow]Description[/]");
            table.AddRow("[lightcyan1]Login[/]", "to login");
            table.AddRow("[lightcyan1]Create coworker[/]", "to create a new employee profile in system");
            table.AddRow("[lightcyan1]Create project[/]", "to create a new project");
            table.AddRow("[lightcyan1]Create task[/]", "to form a new task inside of a project");
            table.AddRow("[lightcyan1]Update coworker[/]", "to change information about an existing employee");
            table.AddRow("[lightcyan1]Update project[/]", "to change information about an existing project");
            table.AddRow("[lightcyan1]Update task[/]", "to change information about an existing task");
            table.AddRow("[lightcyan1]Delete coworker[/]", "to delete a profile on an employee from the system");
            table.AddRow("[lightcyan1]Delete project[/]", "to delete a project");
            table.AddRow("[lightcyan1]Delete task[/]", "to delete a task");
            table.AddRow("[lightcyan1]Read coworkers[/]", "to show all or filtered employee profiles");
            table.AddRow("[lightcyan1]Read projects[/]", "to show all or filtered projects");
            table.AddRow("[lightcyan1]Read tasks[/]", "to show all or filtered tasks");

            while (true)
            {
                AnsiConsole.MarkupLine("[palegreen1_1]Hello! Please login first.[/]");
                var loggedInEmployee = Validator.Login();
                AnsiConsole.MarkupLine("[palegreen1_1]Choose your next action:[/]");
                AnsiConsole.Write(table);

                //продумай связку "действие - категория". Сразу проверять и действие, и категорию (как?), и имеет ли право пользователь?

                AnsiConsole.MarkupLine("[palegreen1_1]Type the name of a category first (tasks, projects or coworkers):[/]");
                var category = Validator.CategoryValidator();
                AnsiConsole.MarkupLine("[lightcyan1]What do you want to do - create, read, update or delete?[/]");
                var operation = Validator.CommandValidator();

                switch(operation)
                {
                    case Commands.Create:
                        if(category == Category.Tasks)
                        {
                            var taskOperations = new Operations<Task>(KeeperOfData.Tasks);
                            var taskCreating = taskOperations.TaskCreate();
                            ProcessingTask processingTask = new(taskCreating);
                            var taskValidated = Validator.TaskValidator<ProcessingTask>(processingTask);
                            if (taskValidated == true)
                            {
                                KeeperOfData.Tasks.Add(processingTask.TransferToTaskItem(processingTask.Name, processingTask.DueDateFinal, processingTask.Description, processingTask.PriorityFinal, processingTask.ProjectFinal, processingTask.ManagerFinal, processingTask.EmployeeFinal));
                                SaveAndLoad<Task>.SaveData(KeeperOfData.Tasks, "tasks.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Task {processingTask.Name} successfully added! [/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Task didn't pass the validator![/]");
                            }
                        }

                        if (category == Category.Projects)
                        {
                            var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                            var projectCreating = projectOperations.ProjectCreate();
                            ProcessingProject processingProject = new(projectCreating);
                            var projectValidated = Validator.ProjectValidator<ProcessingProject>(processingProject);
                            if (projectValidated == true)
                            {
                                KeeperOfData.Projects.Add(processingProject.TransferToProjectItem(processingProject.Name, processingProject.DueDateFinal, processingProject.Description, processingProject.PriorityFinal, processingProject.ManagerFinal));
                                SaveAndLoad<Project>.SaveData(KeeperOfData.Projects, "projects.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Project {processingProject.Name} successfully added! [/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Project didn't pass the validator![/]");
                            }
                        }

                        if (category == Category.Coworkers)
                        {
                            var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                            var coworkerCreating = coworkerOperations.CoworkerCreate();
                            ProcessingCoworker processingCoworker = new(coworkerCreating);
                            var coworkerValidated = Validator.CoworkerValidator<ProcessingCoworker>(processingCoworker);
                            if (coworkerValidated == true)
                            {
                                KeeperOfData.Coworkers.Add(processingCoworker.TransferToCoworkerItem(processingCoworker.Name, processingCoworker.BirthdayFilnal, processingCoworker.EMail, processingCoworker.PositionFinal));
                                SaveAndLoad<Coworker>.SaveData(KeeperOfData.Coworkers, "coworkers.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Employee {processingCoworker.Name} successfully added! [/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Employee didn't pass the validator![/]");
                            }
                        }
                        break;

                    case Commands.Read:
                        if (category == Category.Tasks)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Show all tasks or filtered?:[/]");
                            string filterUserInput = Console.ReadLine();

                            if (filterUserInput != null)
                            {
                                if (filterUserInput == "all".ToLower())
                                {
                                    var taskOperations = new Operations<Task>(KeeperOfData.Tasks);
                                    taskOperations.ShowListOfTasks(KeeperOfData.Tasks);
                                    break;
                                }

                                if (filterUserInput == "filtered".ToLower())
                                {
                                    List<Task> filteredTasks = new List<Task>();
                                    filteredTasks = KeeperOfData.Tasks;

                                    while (true)
                                    {
                                        AnsiConsole.MarkupLine("[palegreen1_1]Filter by project, coworker or priority? Type 'exit' to get out.[/]");
                                        var filter = Validator.FilterValidator();
                                        switch (filter)
                                        {
                                            case ChooseFilter.Project:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the projects name:[/]");
                                                string projectName = Console.ReadLine();
                                                var projectValidated = Validator.ProjectNameValidator(projectName);
                                                if (projectValidated)
                                                {
                                                    filteredTasks = filteredTasks.Where(t => t.Project.Name == projectName).ToList();
                                                    var taskOperations = new Operations<Task>(KeeperOfData.Tasks);
                                                    taskOperations.ShowListOfTasks(filteredTasks);
                                                }
                                                break;

                                            case ChooseFilter.Priority:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the priority (low, medium or high):[/]");
                                                string priorityName = Console.ReadLine();
                                                var priorityValidated = Validator.PriorityValidator(priorityName);
                                                filteredTasks = filteredTasks.Where(t => t.Priority == priorityValidated).ToList();
                                                var taskOperations2 = new Operations<Task>(KeeperOfData.Tasks);
                                                taskOperations2.ShowListOfTasks(filteredTasks);
                                                break;
                                            case ChooseFilter.Coworker:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the employee:[/]");
                                                string coworkerName = Console.ReadLine();
                                                var coworkerValidated = Validator.CoworkerNameValidator(coworkerName);
                                                if (coworkerValidated)
                                                {
                                                    filteredTasks = filteredTasks.Where(t => t.Employee.Name == coworkerName ||
                                                    t.Manager.Name == coworkerName).ToList();
                                                    var taskOperations = new Operations<Task>(KeeperOfData.Tasks);
                                                    taskOperations.ShowListOfTasks(filteredTasks);
                                                }
                                                break;
                                            case ChooseFilter.Exit:
                                                return;
                                        }
                                    }
                                }

                                else
                                {
                                    AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                                }
                            }
                        }

                        if (category == Category.Projects)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Show all projects or filtered?:[/]");
                            string filterUserInput = Console.ReadLine();

                            if (filterUserInput != null)
                            {
                                if (filterUserInput == "all".ToLower())
                                {
                                    var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                                    projectOperations.ShowListOfProjects(KeeperOfData.Projects);
                                    break;
                                }

                                if (filterUserInput == "filtered".ToLower())
                                {
                                    List<Project> filteredProjects = new List<Project>();
                                    filteredProjects = KeeperOfData.Projects;

                                    while (true)
                                    {
                                        AnsiConsole.MarkupLine("[palegreen1_1]Filter by manager or priority? Type 'exit' to get out.[/]");
                                        var filter = Validator.FilterValidator();
                                        switch (filter)
                                        {
                                            case ChooseFilter.Priority:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the priority (low, medium or high):[/]");
                                                string priorityName = Console.ReadLine();
                                                var priorityValidated = Validator.PriorityValidator(priorityName);
                                                filteredProjects = filteredProjects.Where(t => t.Priority == priorityValidated).ToList();
                                                var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                                                projectOperations.ShowListOfProjects(filteredProjects);
                                                break;

                                            case ChooseFilter.Manager:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the manager:[/]");
                                                string coworkerName = Console.ReadLine();
                                                var coworkerValidated = Validator.CoworkerNameValidator(coworkerName);
                                                if (coworkerValidated)
                                                {
                                                    filteredProjects = filteredProjects.Where(t => t.Manager.Name == coworkerName).ToList();
                                                    var projectOperations2 = new Operations<Project>(KeeperOfData.Projects);
                                                    projectOperations2.ShowListOfProjects(filteredProjects);
                                                }
                                                break;

                                            case ChooseFilter.Exit:
                                                return;
                                        }
                                    }

                                }
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                            }
                        }

                        if (category == Category.Coworkers)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Show all coworkers or filtered?:[/]");
                            string filterUserInput = Console.ReadLine();

                            if (filterUserInput != null)
                            {
                                if (filterUserInput == "all".ToLower())
                                {
                                    var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                                    coworkerOperations.ShowListOfCoworkers(KeeperOfData.Coworkers);
                                    break;
                                }

                                if (filterUserInput == "filtered".ToLower())
                                {
                                    List<Coworker> filteredCoworkers = new List<Coworker>();
                                    List<Task> filteredProjects = new List<Task>();
                                    filteredCoworkers = KeeperOfData.Coworkers;
                                    filteredProjects = KeeperOfData.Tasks;

                                    while (true)
                                    {
                                        AnsiConsole.MarkupLine("[palegreen1_1]Filter by position or project? Type 'exit' to get out.[/]");
                                        var filter = Validator.FilterValidator();
                                        switch (filter)
                                        {
                                            case ChooseFilter.Position:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the position (manager, developer or designer):[/]");
                                                string positionName = Console.ReadLine();
                                                var positionValidated = Validator.PositionValidator(positionName);
                                                filteredCoworkers = filteredCoworkers.Where(t => t.Position == positionValidated).ToList();
                                                var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                                                coworkerOperations.ShowListOfCoworkers(filteredCoworkers);
                                                break;

                                            case ChooseFilter.Project:
                                                AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the project:[/]");
                                                string projectName = Console.ReadLine();
                                                var projectValidated = Validator.ProjectNameValidator(projectName);
                                                if (projectValidated)
                                                {
                                                    filteredProjects = filteredProjects.Where(t => t.Project.Name == projectName).ToList();
                                                    filteredCoworkers = filteredProjects.SelectMany(t => new[] { t.Employee, t.Manager }).GroupBy(c => c.Name).Select(g => g.First()).ToList();
                                                    var coworkerOperations2 = new Operations<Coworker>(KeeperOfData.Coworkers);
                                                    coworkerOperations2.ShowListOfCoworkers(filteredCoworkers);
                                                }
                                                break;

                                            case ChooseFilter.Exit:
                                                return;
                                        }
                                    }

                                }
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                            }
                        }
                        break;

                    case Commands.Update:
                        if (category == Category.Tasks)
                        {
                            AnsiConsole.MarkupLine("[darkolivegreen1]Please enter the name os the task you want to update:[/]");
                            string taskNameInput = Console.ReadLine();
                            var updatingTask = KeeperOfData.Tasks.FirstOrDefault(t => t.Name == taskNameInput);
                            if (updatingTask != null)
                            {
                                var taskOperations = new Operations<Task>(KeeperOfData.Tasks);
                                var taskCreating = taskOperations.TaskCreate();
                                ProcessingTask processingTask = new(taskCreating);
                                var taskValidated = Validator.TaskValidator<ProcessingTask>(processingTask);
                                if (taskValidated == true)
                                {
                                    updatingTask.Name = processingTask.Name;
                                    updatingTask.DueDate = processingTask.DueDateFinal;
                                    updatingTask.Description = processingTask.Description;
                                    updatingTask.Priority = processingTask.PriorityFinal;
                                    updatingTask.Project = processingTask.ProjectFinal;
                                    updatingTask.Manager = processingTask.ManagerFinal;
                                    updatingTask.Employee = processingTask.EmployeeFinal;
                                    SaveAndLoad<Task>.SaveData(KeeperOfData.Tasks, "tasks.json");
                                    AnsiConsole.MarkupLine("[darkolivegreen1]Task successfully updated![/]");
                                }
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Task not found![/]");
                            }
                        }

                        if (category == Category.Projects)
                        {
                            AnsiConsole.MarkupLine("[darkolivegreen1]Please enter the name os the project you want to update:[/]");
                            string projectNameInput = Console.ReadLine();
                            var updatingProject = KeeperOfData.Projects.FirstOrDefault(t => t.Name == projectNameInput);
                            if (updatingProject != null)
                            {
                                var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                                var projectCreating = projectOperations.ProjectCreate();
                                ProcessingProject processingProject = new(projectCreating);
                                var projectValidated = Validator.ProjectValidator<ProcessingProject>(processingProject);
                                if (projectValidated == true)
                                {
                                    updatingProject.Name = processingProject.Name;
                                    updatingProject.DueDate = processingProject.DueDateFinal;
                                    updatingProject.Description = processingProject.Description;
                                    updatingProject.Priority = processingProject.PriorityFinal;
                                    updatingProject.Manager = processingProject.ManagerFinal;
                                    SaveAndLoad<Project>.SaveData(KeeperOfData.Projects, "projects.json");
                                    AnsiConsole.MarkupLine("[darkolivegreen1]Project successfully updated![/]");
                                }
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Project not found![/]");
                            }
                        }

                        if (category == Category.Coworkers)
                        {
                            AnsiConsole.MarkupLine("[darkolivegreen1]Please enter the name of the coworker you want to update:[/]");
                            string coworkerNameInput = Console.ReadLine();
                            var updatingCoworker = KeeperOfData.Coworkers.FirstOrDefault(t => t.Name == coworkerNameInput);
                            if (updatingCoworker != null)
                            {
                                var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                                var coworkerUpdating = coworkerOperations.ProjectCreate();
                                ProcessingCoworker processingCoworker = new(coworkerUpdating);
                                var coworkerValidated = Validator.CoworkerValidator<ProcessingCoworker>(processingCoworker);
                                if (coworkerValidated == true)
                                {
                                    updatingCoworker.Name = processingCoworker.Name;
                                    updatingCoworker.Birthday = processingCoworker.BirthdayFilnal;
                                    updatingCoworker.EMail = processingCoworker.EMail;
                                    updatingCoworker.Position = processingCoworker.PositionFinal;
                                    SaveAndLoad<Coworker>.SaveData(KeeperOfData.Coworkers, "coworkers.json");
                                    AnsiConsole.MarkupLine("[darkolivegreen1]Coworker successfully updated![/]");
                                }
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Coworker not found![/]");
                            }
                        }
                        break;

                    case Commands.Delete:
                        if (category == Category.Tasks)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the task you want to delete:[/]");
                            string deleteUserInput = Console.ReadLine();
                            var validating = Validator.TaskNameValidator(deleteUserInput);
                            if (validating)
                            {
                                var taskToDelete = KeeperOfData.Tasks.FirstOrDefault(t => t.Name == deleteUserInput);
                                KeeperOfData.Tasks.Remove(taskToDelete);
                                SaveAndLoad<Task>.SaveData(KeeperOfData.Tasks, "tasks.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Task {taskToDelete.Name} successfully removed! [/]");
                                break;
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Task name didn't pass the validator![/]");
                            }
                        }

                        if (category == Category.Projects)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the project you want to delete:[/]");
                            string deleteUserInput = Console.ReadLine();
                            var validating = Validator.ProjectNameValidator(deleteUserInput);
                            if (validating)
                            {
                                var projectToDelete = KeeperOfData.Projects.FirstOrDefault(t => t.Name == deleteUserInput);
                                KeeperOfData.Projects.Remove(projectToDelete);
                                SaveAndLoad<Project>.SaveData(KeeperOfData.Projects, "projects.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Project {projectToDelete.Name} successfully removed![/]");
                                break;
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Project name didn't pass the validator![/]");
                            }
                        }

                        if (category == Category.Coworkers)
                        {
                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the coworker you want to delete:[/]");
                            string deleteUserInput = Console.ReadLine();
                            var validating = Validator.CoworkerNameValidator(deleteUserInput);
                            if (validating)
                            {
                                var coworkerToDelete = KeeperOfData.Coworkers.FirstOrDefault(t => t.Name == deleteUserInput);
                                KeeperOfData.Coworkers.Remove(coworkerToDelete);
                                SaveAndLoad<Coworker>.SaveData(KeeperOfData.Coworkers, "coworkers.json");
                                AnsiConsole.MarkupLine($"[darkolivegreen1] Project {coworkerToDelete.Name} successfully removed![/]");
                                break;
                            }

                            else
                            {
                                AnsiConsole.MarkupLine("[magenta1]Project name didn't pass the validator![/]");
                            }
                        }
                        break;
                }
            }
        }
                
    }
}
            