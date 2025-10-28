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

        //Create:
        public List<string> TaskCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the task, each supplemented with '':[/]");
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

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
            }
            return tasks;
        }

        public ProcessingTask FormingNewTask<T>(List<string> newItem)
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

        public List<string> ProjectCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the new project, each supplemented with '':[/]");
            AnsiConsole.MarkupLine("[darkolivegreen1] 'name' 'dd.mm.yyyy (due date)' 'description' 'priority (low, medium or high)' 'manager'[/]");

            string projectInput = Console.ReadLine();
            List<string> projects = new List<string>();

            try
            {
                var matches = Regex.Matches(projectInput, "'([^']*)'|(\\S+)");
                foreach (Match match in matches)
                {
                    projects.Add(match.Value);
                }

                FormingNewProject<ProcessingTask>(projects);
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
            }
            return projects;
        }

        public ProcessingProject FormingNewProject<T>(List<string> newItem)
        {
            ProcessingProject processingProject = new ProcessingProject(newItem);
            for (int i = 0; i < newItem.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        processingProject.Name = newItem[i];
                        break;
                    case 1:
                        processingProject.DueDate = newItem[i];
                        break;
                    case 2:
                        processingProject.Description = newItem[i];
                        break;
                    case 3:
                        processingProject.Priority = newItem[i];
                        break;
                    case 4:
                        processingProject.Manager = newItem[i];
                        break;
                }
            }
            return processingProject;
        }

        public List<string> CoworkerCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the new coworker, each supplemented with '':[/]");
            AnsiConsole.MarkupLine("[darkolivegreen1] 'name' 'dd.mm.yyyy (birthday)' 'e-mail' 'position (Manager, Developer or Designer)' [/]");

            string coworkerInput = Console.ReadLine();
            List<string> coworkers = new List<string>();

            try
            {
                var matches = Regex.Matches(coworkerInput, "'([^']*)'|(\\S+)");
                foreach (Match match in matches)
                {
                    coworkers.Add(match.Value);
                }

                FormingNewCoworker<ProcessingCoworker>(coworkers);
            }

            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
            }
            return coworkers;
        }

        public ProcessingCoworker FormingNewCoworker<T>(List<string> newItem)
        {
            ProcessingCoworker processingCoworker = new ProcessingCoworker(newItem);
            for (int i = 0; i < newItem.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        processingCoworker.Name = newItem[i];
                        break;
                    case 1:
                        processingCoworker.Birthday = newItem[i];
                        break;
                    case 2:
                        processingCoworker.EMail = newItem[i];
                        break;
                    case 3:
                        processingCoworker.Position = newItem[i];
                        break;
                }
            }
            return processingCoworker;
        }
        //Read:
        public void ShowListOfTasks (List<Task> tasks)
        {
            var table = new Table();
            table.Title = new TableTitle("[yellow]All Tasks[/]");
            table.Border = TableBorder.Rounded;
            table.AddColumn("Name");
            table.AddColumn("Deadline");
            table.AddColumn("Description");
            table.AddColumn("Priority");
            table.AddColumn("Project");
            table.AddColumn("Manager");
            table.AddColumn("Employee");

            foreach (var task in tasks)
            {
                table.AddRow(
                    task.Name ?? "",
                    task.DueDate.ToString(),
                    task.Description ?? "",
                    task.Priority.ToString(),
                    task.Project.Name,
                    task.Manager.Name,
                    task.Employee.Name
                    );
            }
            AnsiConsole.Write(table);
        }

        public void ShowListOfProjects(List<Project> projects)
        {
            var table = new Table();
            table.Title = new TableTitle("[yellow]All Projects[/]");
            table.Border = TableBorder.Rounded;
            table.AddColumn("Name");
            table.AddColumn("Deadline");
            table.AddColumn("Description");
            table.AddColumn("Priority");
            table.AddColumn("Manager");

            foreach (var project in projects)
            {
                table.AddRow(
                    project.Name ?? "",
                    project.DueDate.ToString(),
                    project.Description ?? "",
                    project.Priority.ToString(),
                    project.Manager.Name
                    );
            }
            AnsiConsole.Write(table);

        }

        public void ShowListOfCoworkers(List<Coworker> coworkers)
        {
            var table = new Table();
            table.Title = new TableTitle("[yellow]All Coworkers[/]");
            table.Border = TableBorder.Rounded;
            table.AddColumn("Name");
            table.AddColumn("Birthday");
            table.AddColumn("E-Mail");
            table.AddColumn("Position");

            foreach (var coworker in coworkers)
            {
                table.AddRow(
                    coworker.Name ?? "",
                    coworker.Birthday.ToString(),
                    coworker.EMail ?? "",
                    coworker.Position.ToString()
                    );
            }
            AnsiConsole.Write(table);

        }

    }
}
