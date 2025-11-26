using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskTracking
{
    public static class Operations
    {
        private static readonly string connection = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=ILove6Bo0bs;Include Error Detail=true;";
        public static CoworkerRepo coworkerRepo = new CoworkerRepo(connection);
        public static ProjectRepo projectRepo = new ProjectRepo(connection);
        public static TaskRepo taskRepo = new TaskRepo(connection);

        public static Table allCoworkers = AllToTable(coworkerRepo.GetAllCoworkers());
        public static Table allProjects = AllToTable(projectRepo.GetAllProjects());
        public static Table allTasks = AllToTable(taskRepo.GetAllTasks());

        public static bool ProjectDataToDB (List<string> data)
        {
            bool success = false;
            ProjectDTO projectDTO = new ProjectDTO();
            Project project = null;
            DateTime validDate;
            Priority validPriority;
            Position validPosition;

            //delete project:
            if (data.Count == 1)
            {
                int id = 0;
                int.TryParse(data[0], out id);
                projectDTO.Id = id;
                project = new Project
                {
                    Id = id
                };
                projectRepo.DeleteProject(project);
                success = true;
            }
            
            //create project:
            if (data.Count == 5)
            {
                DateTime.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
                Enum.TryParse<Priority>(data[3], ignoreCase: true, out validPriority);

                projectDTO.Name = data[0];
                projectDTO.DueDate = validDate;
                projectDTO.Description = data[2];
                projectDTO.Priority = validPriority;
                int id = 0;
                int.TryParse(data[4], out id);
                projectDTO.ManagerId = id;

                project = new Project
                {
                    Name = projectDTO.Name,
                    DueDate = validDate,
                    Description = projectDTO.Description,
                    Priority = validPriority,
                    ManagerId = projectDTO.ManagerId
                };
                projectRepo.CreateProject(project);
                success = true;
            }

            //update project:
            if (data.Count == 6)
            {
                int id = int.Parse(data[0]);
                project = projectRepo.GetProjectById(id);
                projectDTO.Id = project.Id;
                projectDTO.Name = project.Name;
                projectDTO.DueDate = project.DueDate;
                projectDTO.Description = project.Description;
                projectDTO.Priority = project.Priority;
                projectDTO.ManagerId = project.ManagerId;

                DateTime? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTime.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var parsedDate))
                {
                    validDateUpdate = parsedDate;
                }

                Priority? validPrioUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[4]) &&
                    Enum.TryParse<Priority>(data[4], true, out var priority))
                {
                    validPrioUpdate = priority;
                }

                projectDTO.Name = string.IsNullOrWhiteSpace(data[1]) ? projectDTO.Name : data[1];
                projectDTO.DueDate = validDateUpdate ?? projectDTO.DueDate;
                projectDTO.Description = string.IsNullOrWhiteSpace(data[3]) ? projectDTO.Description : data[3];
                projectDTO.Priority = validPrioUpdate ?? projectDTO.Priority;
                projectDTO.ManagerId = string.IsNullOrWhiteSpace(data[5]) ? projectDTO.ManagerId : int.Parse(data[5]);

                project = new Project
                {
                    Id = projectDTO.Id,
                    Name = projectDTO.Name,
                    DueDate = project.DueDate,
                    Description = projectDTO.Description,
                    Priority = projectDTO.Priority,
                    ManagerId = projectDTO.ManagerId
                };
                projectRepo.UpdateProject(project);
                success = true;
            }
            return success;
        }
        public static bool TaskDataToDB (List<string> data)
        {
            bool success = false;
            TaskDTO taskDTO = new TaskDTO();
            Task task = null;
            DateTime validDate;
            Priority validPriority;
            Position validPosition;

            //delete task:
            if (data.Count == 1)
            {
                int id = 0;
                int.TryParse(data[0], out id);
                taskDTO.Id = id;
                task = new Task()
                { 
                    Id = taskDTO.Id
                };
                taskRepo.DeleteTask(task);
                success = true;
            }

            //create task:
            if(data.Count == 7)
            {
                DateTime.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
                Enum.TryParse<Priority>(data[3], ignoreCase: true, out validPriority);

                taskDTO.Name = data[0];
                taskDTO.DueDate = validDate;
                taskDTO.Description = data[2];
                taskDTO.Priority = validPriority;
                int idProj = 0;
                int.TryParse(data[4], out idProj);
                taskDTO.ProjectId = idProj;
                int idMan = 0;
                int.TryParse(data[5], out idMan);
                taskDTO.ManagerId = idMan;
                int idEmp = 0;
                int.TryParse(data[6], out idEmp);
                taskDTO.EmployeeId = idEmp;

                task = new Task
                {
                    Name = taskDTO.Name,
                    DueDate = validDate,
                    Description = taskDTO.Description,
                    Priority = validPriority,
                    ProjectId = taskDTO.ProjectId,
                    ManagerId = taskDTO.ManagerId,
                    EmployeeId = taskDTO.EmployeeId
                };
                taskRepo.CreateTask(task);
                success = true;
            }

            //update task:
            if (data.Count == 8)
            {
                int id = int.Parse(data[0]);
                task = taskRepo.GetTaskById(id);
                taskDTO.Id = task.Id;
                taskDTO.Name = task.Name;
                taskDTO.DueDate = task.DueDate;
                taskDTO.Description = task.Description;
                taskDTO.Priority = task.Priority;
                taskDTO.ProjectId = task.ProjectId;
                taskDTO.ManagerId = task.ManagerId;
                taskDTO.EmployeeId = task.EmployeeId;

                DateTime? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTime.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var parsedDate))
                {
                    validDateUpdate = parsedDate;
                }

                Priority? validPrioUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[4]) &&
                    Enum.TryParse<Priority>(data[4], true, out var priority))
                {
                    validPrioUpdate = priority;
                }

                taskDTO.Name = string.IsNullOrWhiteSpace(data[1]) ? taskDTO.Name : data[1];
                taskDTO.DueDate = validDateUpdate ?? taskDTO.DueDate;
                taskDTO.Description = string.IsNullOrWhiteSpace(data[3]) ? taskDTO.Description : data[3];
                taskDTO.Priority = validPrioUpdate ?? taskDTO.Priority;
                taskDTO.ProjectId = string.IsNullOrWhiteSpace(data[5]) ? taskDTO.ProjectId : int.Parse(data[5]);
                taskDTO.ManagerId = string.IsNullOrWhiteSpace(data[6]) ? taskDTO.ManagerId : int.Parse(data[6]);
                taskDTO.EmployeeId = string.IsNullOrWhiteSpace(data[7]) ? taskDTO.EmployeeId : int.Parse(data[7]);

                task = new Task
                {
                    Id = taskDTO.Id,
                    Name = taskDTO.Name,
                    DueDate = taskDTO.DueDate,
                    Description = taskDTO.Description,
                    Priority = taskDTO.Priority,
                    ProjectId = taskDTO.ProjectId,
                    ManagerId = taskDTO.ManagerId,
                    EmployeeId = taskDTO.EmployeeId
                };
                taskRepo.UpdateTask(task);
                success = true;
            }

            return success;
        }
        public static bool CoworkerDataToDB(List<string> data)
        {
            bool success = false;
            CoworkerDTO coworkerDTO = new CoworkerDTO();
            Coworker coworker = null;
            DateTime validDate = default;
            Position validPosition = default;

            //delete coworker:
            if (data.Count == 2)
            {
                coworkerDTO.EMail = data[0];
                coworkerDTO.Password = BCrypt.Net.BCrypt.HashPassword(data[1]);

                coworker = new Coworker
                {
                    EMail = coworkerDTO.EMail,
                    Password = coworkerDTO.Password
                };
                coworkerRepo.DeleteCoworker(coworker);
                success = true;
            }

            //create coworker:
            if (data.Count == 5)
            {
                DateTime.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
                Enum.TryParse<Position>(data[3], ignoreCase: true, out validPosition);

                coworkerDTO.Name = data[0];
                coworkerDTO.Birthday = validDate;
                coworkerDTO.EMail = data[2];
                coworkerDTO.Position = validPosition;
                coworkerDTO.Password = BCrypt.Net.BCrypt.HashPassword(data[4]);

                coworker = new Coworker
                {
                    Name = coworkerDTO.Name,
                    Birthday = coworkerDTO.Birthday,
                    EMail = coworkerDTO.EMail,
                    Position = coworkerDTO.Position,
                    Password = coworkerDTO.Password
                };
                coworkerRepo.CreateCoworker(coworker);
                success = true;
            }

            //update coworker:
            if (data.Count == 6)
            {
                int id = int.Parse(data[0]);
                coworker = coworkerRepo.GetCoworkerById(id);
                coworkerDTO.Id = coworker.Id;
                coworkerDTO.Name = coworker.Name;
                coworkerDTO.Birthday = coworker.Birthday;
                coworkerDTO.EMail = coworker.EMail;
                coworkerDTO.Position = coworker.Position;
                coworker.Password = coworker.Password;

                DateTime? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTime.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var parsedDate))
                {
                    validDateUpdate = parsedDate;
                }

                Position? validPositionUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[4]) &&
                    Enum.TryParse<Position>(data[4], true, out var position))
                {
                    validPositionUpdate = position;
                }

                coworkerDTO.Name = string.IsNullOrWhiteSpace(data[1]) ? coworkerDTO.Name : data[1];
                coworkerDTO.Birthday = validDateUpdate ?? coworkerDTO.Birthday;
                coworkerDTO.EMail = string.IsNullOrWhiteSpace(data[3]) ? coworkerDTO.EMail : data[3];
                coworkerDTO.Position = validPositionUpdate ?? coworkerDTO.Position;

                coworkerDTO.Password = string.IsNullOrWhiteSpace(data[5]) ? coworkerDTO.Password : BCrypt.Net.BCrypt.HashPassword(data[5]);

                coworkerRepo.UpdateCoworker(new Coworker
                {
                    Id = coworkerDTO.Id,
                    Name = coworkerDTO.Name,
                    Birthday = coworkerDTO.Birthday,
                    EMail = coworkerDTO.EMail,
                    Position = coworkerDTO.Position,
                    Password = coworkerDTO.Password
                });

                success = true;
            }

            return success;
        }
        public static Table ShowAll(Category? category)
        {
            switch (category)
            {
                case Category.Coworkers:
                    return allCoworkers = AllToTable(coworkerRepo.GetAllCoworkers());

                case Category.Projects:
                    return allProjects = AllToTable(projectRepo.GetAllProjects());

                case Category.Tasks:
                    return allTasks = AllToTable(taskRepo.GetAllTasks());

                default:
                    throw new NotSupportedException($"Category {category} is not supported.");
            }
        }
        private static Table AllToTable<T>(List<T> list)
        {
            var table = new Table();

            switch (list)
            {
                case List<CoworkerDTO> coworkerDTOs:
                    table.AddColumn("[bold yellow]Id[/]");
                    table.AddColumn("[bold yellow]Name[/]");
                    table.AddColumn("[bold yellow]Birthday[/]");
                    table.AddColumn("[bold yellow]Email[/]");
                    table.AddColumn("[bold yellow]Position[/]");

                    foreach (var dto in coworkerDTOs)
                    {
                        table.AddRow($"{dto.Id}",
                        $"{dto.Name}",
                        $"{dto.Birthday:d}",
                        $"{dto.EMail}",
                        $"{dto.Position}");
                    }
                    break;

                case List<ProjectDTO> projectDTOs:
                    table.AddColumn("[bold yellow]Id[/]");
                    table.AddColumn("[bold yellow]Name[/]");
                    table.AddColumn("[bold yellow]DueDate[/]");
                    table.AddColumn("[bold yellow]Description[/]");
                    table.AddColumn("[bold yellow]Priority[/]");
                    table.AddColumn("[bold yellow]ManagersId[/]");

                    foreach (var dto in projectDTOs)
                    {
                        table.AddRow($"{dto.Id}",
                        $"{dto.Name}",
                        $"{dto.DueDate}",
                        $"{dto.Description}",
                        $"{dto.Priority}",
                        $"{dto.ManagerId}");
                    }
                    break;

                case List<TaskDTO> taskDTOs:
                    table.AddColumn("[bold yellow]Id[/]");
                    table.AddColumn("[bold yellow]Name[/]");
                    table.AddColumn("[bold yellow]DueDate[/]");
                    table.AddColumn("[bold yellow]Description[/]");
                    table.AddColumn("[bold yellow]Priority[/]");
                    table.AddColumn("[bold yellow]ProjectId[/]");
                    table.AddColumn("[bold yellow]ManagersId[/]");
                    table.AddColumn("[bold yellow]EmployeeId[/]");

                    foreach (var dto in taskDTOs)
                    {
                        table.AddRow($"{dto.Id}",
                        $"{dto.Name}",
                        $"{dto.DueDate}",
                        $"{dto.Description}",
                        $"{dto.Priority}",
                        $"{dto.ProjectId}",
                        $"{dto.ManagerId}",
                        $"{dto.EmployeeId}");
                    }
                    break;
            }
            return table;
        }
        public static Table CoworkerFiltered(List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            filters = DataToDictionary(filterOptions, filterOptionsValues);
            List<CoworkerDTO> coworkerDTOs = coworkerRepo.GetFilteredCoworkers(filters);
            var tableCoworkers = AllToTable(coworkerDTOs);
            return tableCoworkers;
        }
        public static Table ProjectsFiltered(List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            filters = DataToDictionary(filterOptions, filterOptionsValues);
            List<ProjectDTO> projectDTOs = projectRepo.GetFilteredProjects(filters);
            var tableProjects = AllToTable(projectDTOs);
            return tableProjects;
        }
        public static Table TasksFiltered(List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            filters = DataToDictionary(filterOptions, filterOptionsValues);
            List<TaskDTO> taskDTOs = taskRepo.GetFilteredTasks(filters);
            var tableProjects = AllToTable(taskDTOs);
            return tableProjects;  
        }
        private static Dictionary<FilterOptions, string> DataToDictionary (List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            if (filterOptions.Count != filterOptionsValues.Count)
            {
                AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
            }
            for (int i = 0; i < filterOptions.Count; i++)
            {
                filters[filterOptions[i]] = filterOptionsValues[i];
            }
            return filters;
        }
    }
}
