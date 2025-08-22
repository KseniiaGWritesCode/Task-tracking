using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class AppController
    {
        Validator programValidator = new Validator();
        public Category Category;
        public ChooseOperation Operation;
        public ChooseFilter Filter;

        public AppController()
        {
            KeeperOfData.Tasks = SaveAndLoad<TaskItem>.LoadData("tasks.json");
            KeeperOfData.Projects = SaveAndLoad<Project>.LoadData("projects.json");
            KeeperOfData.Coworkers = SaveAndLoad<Coworker>.LoadData("coworkers.json");
            //SaveAndLoad<TaskItem>.SaveData(KeeperOfData.Tasks, "tasks.json");
            //SaveAndLoad<Project>.SaveData(KeeperOfData.Projects, "projects.json");
            //SaveAndLoad<Coworker>.SaveData(KeeperOfData.Coworkers, "coworkers.json");

            Category = programValidator.CategoryValidator();
            Operation = programValidator.OperationValidator();

            switch (Operation)
            {
                case ChooseOperation.create:

                    if(Category == Category.tasks)
                    {
                        var taskOperations = new Operations<TaskItem>(KeeperOfData.Tasks);
                        var taskCreating = taskOperations.TaskCreate();
                        ProcessingTask processingTask = new(taskCreating);
                        var taskValidated = programValidator.TaskValidator<ProcessingTask>(processingTask);
                        if (taskValidated == true)
                        {
                            KeeperOfData.Tasks.Add(processingTask.TransferToTaskItem(processingTask.Name, processingTask.DueDateFinal, processingTask.Description, processingTask.PriorityFinal, processingTask.ProjectFinal, processingTask.ManagerFinal, processingTask.EmployeeFinal));
                            SaveAndLoad<TaskItem>.SaveData(KeeperOfData.Tasks, "tasks.json");
                            AnsiConsole.MarkupLine($"[darkolivegreen1] Task {processingTask.Name} successfully added! [/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[magenta1]Task didn't pass the validator![/]");
                        }
                    }

                    if (Category == Category.projects)
                    {
                        var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                        var projectCreating = projectOperations.ProjectCreate();
                        ProcessingProject processingProject = new(projectCreating);
                        var projectValidated = programValidator.ProjectValidator<ProcessingProject>(processingProject);
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

                    if (Category == Category.coworkers)
                    {
                        var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                        var coworkerCreating = coworkerOperations.CoworkerCreate();
                        ProcessingCoworker processingCoworker = new(coworkerCreating);
                        var coworkerValidated = programValidator.CoworkerValidator<ProcessingCoworker>(processingCoworker);
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

                case ChooseOperation.read:

                    if(Category == Category.tasks)
                    {
                        AnsiConsole.MarkupLine("[palegreen1_1]Show all tasks or filtered?:[/]");
                        string filterUserInput = Console.ReadLine();

                        if (filterUserInput != null)
                        {
                            if (filterUserInput == "all".ToLower())
                            {
                                var taskOperations = new Operations<TaskItem>(KeeperOfData.Tasks);
                                taskOperations.ShowListOfTasks(KeeperOfData.Tasks);
                                break;
                            }

                            if (filterUserInput == "filtered".ToLower())
                            {
                                List<TaskItem> filteredTasks = new List<TaskItem>();
                                filteredTasks = KeeperOfData.Tasks;

                                while (true)
                                {
                                    AnsiConsole.MarkupLine("[palegreen1_1]Filter by project, coworker or priority? Type 'exit' to get out.[/]");
                                    Filter = programValidator.FilterValidator();
                                    switch (Filter)
                                    {
                                        case ChooseFilter.project:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the projects name:[/]");
                                            string projectName = Console.ReadLine();
                                            var projectValidated = programValidator.ProjectNameValidator(projectName);
                                            if (projectValidated)
                                            {
                                                filteredTasks = filteredTasks.Where(t => t.Project.Name == projectName).ToList();
                                                var taskOperations = new Operations<TaskItem>(KeeperOfData.Tasks);
                                                taskOperations.ShowListOfTasks(filteredTasks);
                                            }
                                            break;

                                        case ChooseFilter.priority:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the priority (low, medium or high):[/]");
                                            string priorityName = Console.ReadLine();
                                            var priorityValidated = programValidator.PriorityValidator(priorityName);
                                            filteredTasks = filteredTasks.Where(t => t.Priority == priorityValidated).ToList();
                                            var taskOperations2 = new Operations<TaskItem>(KeeperOfData.Tasks);
                                            taskOperations2.ShowListOfTasks(filteredTasks);
                                            break;
                                        case ChooseFilter.coworker:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the employee:[/]");
                                            string coworkerName = Console.ReadLine();
                                            var coworkerValidated = programValidator.CoworkerNameValidator(coworkerName);
                                            if (coworkerValidated)
                                            {
                                                filteredTasks = filteredTasks.Where(t => t.Employee.Name == coworkerName ||
                                                t.Manager.Name == coworkerName).ToList();
                                                var taskOperations = new Operations<TaskItem>(KeeperOfData.Tasks);
                                                taskOperations.ShowListOfTasks(filteredTasks);
                                            }
                                            break;
                                        case ChooseFilter.exit:
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

                    if(Category == Category.projects)
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
                                    Filter = programValidator.FilterValidator();
                                    switch (Filter)
                                    {
                                        case ChooseFilter.priority:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the priority (low, medium or high):[/]");
                                            string priorityName = Console.ReadLine();
                                            var priorityValidated = programValidator.PriorityValidator(priorityName);
                                            filteredProjects = filteredProjects.Where(t => t.Priority == priorityValidated).ToList();
                                            var projectOperations = new Operations<Project>(KeeperOfData.Projects);
                                            projectOperations.ShowListOfProjects(filteredProjects);
                                            break;

                                        case ChooseFilter.manager:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the manager:[/]");
                                            string coworkerName = Console.ReadLine();
                                            var coworkerValidated = programValidator.CoworkerNameValidator(coworkerName);
                                            if (coworkerValidated)
                                            {
                                                filteredProjects = filteredProjects.Where(t => t.Manager.Name == coworkerName).ToList();
                                                var projectOperations2 = new Operations<Project>(KeeperOfData.Projects);
                                                projectOperations2.ShowListOfProjects(filteredProjects);
                                            }
                                            break;

                                        case ChooseFilter.exit:
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

                    if (Category == Category.coworkers)
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
                                List<TaskItem> filteredProjects = new List<TaskItem>();
                                filteredCoworkers = KeeperOfData.Coworkers;
                                filteredProjects = KeeperOfData.Tasks;

                                while (true)
                                {
                                    AnsiConsole.MarkupLine("[palegreen1_1]Filter by position or project? Type 'exit' to get out.[/]");
                                    Filter = programValidator.FilterValidator();
                                    switch (Filter)
                                    {
                                        case ChooseFilter.position:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the position (manager, developer or designer):[/]");
                                            string positionName = Console.ReadLine();
                                            var positionValidated = programValidator.PositionValidator(positionName);
                                            filteredCoworkers = filteredCoworkers.Where(t => t.Position == positionValidated).ToList();
                                            var coworkerOperations = new Operations<Coworker>(KeeperOfData.Coworkers);
                                            coworkerOperations.ShowListOfCoworkers(filteredCoworkers);
                                            break;

                                        case ChooseFilter.project:
                                            AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the project:[/]");
                                            string projectName = Console.ReadLine();
                                            var projectValidated = programValidator.ProjectNameValidator(projectName);
                                            if (projectValidated)
                                            {
                                                filteredProjects = filteredProjects.Where(t => t.Project.Name == projectName).ToList();
                                                filteredCoworkers = filteredProjects.SelectMany(t => new[] { t.Employee, t.Manager }).GroupBy(c => c.Name).Select(g => g.First()).ToList();
                                                var coworkerOperations2 = new Operations<Coworker>(KeeperOfData.Coworkers);
                                                coworkerOperations2.ShowListOfCoworkers(filteredCoworkers);
                                            }
                                            break;

                                        case ChooseFilter.exit:
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

                case ChooseOperation.delete:
                    if (Category == Category.tasks)
                    {
                        AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the task you want to delete:[/]");
                        string deleteUserInput = Console.ReadLine();
                        var validating = programValidator.TaskNameValidator(deleteUserInput);
                        if (validating)
                        {
                            var taskToDelete = KeeperOfData.Tasks.FirstOrDefault(t => t.Name == deleteUserInput);
                            KeeperOfData.Tasks.Remove(taskToDelete);
                            SaveAndLoad<TaskItem>.SaveData(KeeperOfData.Tasks, "tasks.json");
                            AnsiConsole.MarkupLine($"[darkolivegreen1] Task {taskToDelete.Name} successfully removed! [/]");
                            break;
                        }

                        else
                        {
                            AnsiConsole.MarkupLine("[magenta1]Task name didn't pass the validator![/]");
                        }
                    }

                    if (Category == Category.projects)
                    {
                        AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the project you want to delete:[/]");
                        string deleteUserInput = Console.ReadLine();
                        var validating = programValidator.ProjectNameValidator(deleteUserInput);
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

                    if (Category == Category.coworkers)
                    {
                        AnsiConsole.MarkupLine("[palegreen1_1]Please type the name of the coworker you want to delete:[/]");
                        string deleteUserInput = Console.ReadLine();
                        var validating = programValidator.CoworkerNameValidator(deleteUserInput);
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
