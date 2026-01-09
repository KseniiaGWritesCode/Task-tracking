using Microsoft.VisualBasic;
using Npgsql;
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
        public static bool ProjectDataToDB (List<string> data)
        {
            bool success = false;
            ProjectDTO projectDTO = new ProjectDTO();
            Project project = null;
            DateTimeOffset validDate;
            Priority validPriority;
            Position validPosition;

            //delete project:
            if (data.Count == 1)
            {
                int id = 0;
                int.TryParse(data[0], out id);
                project = Initializer.GetDbContext().Projects.FirstOrDefault(p => p.Id == id);
                if (project == null) throw new InvalidOperationException($"Project {id} not found");
                //projectDTO.Id = id;
                //project = new Project
                //{
                //    Id = id
                //};
                Initializer.GetDbContext().Projects.Remove(project);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }
            
            //create project:
            if (data.Count == 5)
            {
                DateTimeOffset.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
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
                    DueDate = validDate.ToUniversalTime(),
                    Description = projectDTO.Description,
                    Priority = validPriority,
                    ManagerId = projectDTO.ManagerId,
                    Manager = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == projectDTO.ManagerId)
                };
                Initializer.GetDbContext().Projects.Add(project);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            //update project:
            if (data.Count == 6)
            {
                int id = int.Parse(data[0]);
                project = Initializer.GetDbContext().Projects.FirstOrDefault(p => p.Id == id);
                if (project == null) throw new InvalidOperationException($"Project {id} not found");
                projectDTO.Id = project.Id;
                projectDTO.Name = project.Name;
                projectDTO.DueDate = project.DueDate.ToLocalTime();
                projectDTO.Description = project.Description;
                projectDTO.Priority = project.Priority;
                projectDTO.ManagerId = project.ManagerId;

                DateTimeOffset? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTimeOffset.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
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
                    DueDate = project.DueDate.ToUniversalTime(),
                    Description = projectDTO.Description,
                    Priority = projectDTO.Priority,
                    ManagerId = projectDTO.ManagerId,
                    Manager = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == projectDTO.ManagerId)
                };
                Initializer.GetDbContext().Projects.Update(project);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }
            return success;
        }
        public static bool TaskDataToDB (List<string> data)
        {
            bool success = false;
            TaskDTO taskDTO = new TaskDTO();
            Task task = null;
            DateTimeOffset validDate;
            Priority validPriority;
            Position validPosition;

            //delete task:
            if (data.Count == 1)
            {
                int id = 0;
                int.TryParse(data[0], out id);
                task = Initializer.GetDbContext().Tasks.FirstOrDefault(p => p.Id == id);
                if (task == null) throw new InvalidOperationException($"Task {id} not found");
                //taskDTO.Id = id;
                //task = new Task()
                //{ 
                //    Id = taskDTO.Id
                //};
                Initializer.GetDbContext().Tasks.Remove(task);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            //create task:
            if(data.Count == 7)
            {
                DateTimeOffset.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
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
                    DueDate = validDate.ToUniversalTime(),
                    Description = taskDTO.Description,
                    Priority = validPriority,
                    ProjectId = taskDTO.ProjectId,
                    Project = Initializer.GetDbContext().Projects.FirstOrDefault(p => p.Id == taskDTO.ProjectId),
                    ManagerId = taskDTO.ManagerId,
                    Manager = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == taskDTO.ManagerId),
                    EmployeeId = taskDTO.EmployeeId,
                    Employee = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == taskDTO.EmployeeId)
                };
                Initializer.GetDbContext().Tasks.Add(task);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            //update task:
            if (data.Count == 8)
            {
                int id = int.Parse(data[0]);
                task = Initializer.GetDbContext().Tasks.FirstOrDefault(p => p.Id == id);
                if (task == null) throw new InvalidOperationException($"Task {id} not found");
                taskDTO.Id = task.Id;
                taskDTO.Name = task.Name;
                taskDTO.DueDate = task.DueDate.ToLocalTime();
                taskDTO.Description = task.Description;
                taskDTO.Priority = task.Priority;
                taskDTO.ProjectId = task.ProjectId;
                taskDTO.ManagerId = task.ManagerId;
                taskDTO.EmployeeId = task.EmployeeId;

                DateTimeOffset? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTimeOffset.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
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
                    DueDate = taskDTO.DueDate.ToUniversalTime(),
                    Description = taskDTO.Description,
                    Priority = taskDTO.Priority,
                    ProjectId = taskDTO.ProjectId,
                    Project = Initializer.GetDbContext().Projects.FirstOrDefault(p => p.Id == taskDTO.ProjectId),
                    ManagerId = taskDTO.ManagerId,
                    Manager = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == taskDTO.ManagerId),
                    EmployeeId = taskDTO.EmployeeId,
                    Employee = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == taskDTO.EmployeeId)
                };
                Initializer.GetDbContext().Tasks.Update(task);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            return success;
        }
        public static bool CoworkerDataToDB(List<string> data)
        {
            bool success = false;
            CoworkerDTO coworkerDTO = new CoworkerDTO();
            Coworker coworker = null;
            DateTimeOffset validDate = default;
            Position validPosition = default;

            //delete coworker:
            if (data.Count == 2)
            {
                coworker = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.EMail == data[0] && p.Password == BCrypt.Net.BCrypt.HashPassword(data[1]));
                if (coworker == null) throw new InvalidOperationException($"Coworker {data[0]} not found");

                coworkerDTO.EMail = data[0];
                coworkerDTO.Password = BCrypt.Net.BCrypt.HashPassword(data[1]);

                //coworker = new Coworker
                //{
                //    EMail = coworkerDTO.EMail,
                //    Password = coworkerDTO.Password
                //};
                Initializer.GetDbContext().Coworkers.Remove(coworker);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            //create coworker:
            if (data.Count == 5)
            {
                DateTimeOffset.TryParseExact(data[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);
                Enum.TryParse<Position>(data[3], ignoreCase: true, out validPosition);

                coworkerDTO.Name = data[0];
                coworkerDTO.Birthday = validDate;
                coworkerDTO.EMail = data[2];
                coworkerDTO.Position = validPosition;
                coworkerDTO.Password = BCrypt.Net.BCrypt.HashPassword(data[4]);

                coworker = new Coworker
                {
                    Name = coworkerDTO.Name,
                    Birthday = coworkerDTO.Birthday.ToUniversalTime(),
                    EMail = coworkerDTO.EMail,
                    Position = coworkerDTO.Position,
                    Password = coworkerDTO.Password
                };
                Initializer.GetDbContext().Coworkers.Add(coworker);
                Initializer.GetDbContext().SaveChanges();
                success = true;
            }

            //update coworker:
            if (data.Count == 6)
            {
                int id = int.Parse(data[0]);
                coworker = Initializer.GetDbContext().Coworkers.FirstOrDefault(p => p.Id == id);
                if (coworker == null) throw new InvalidOperationException($"Coworker {id} not found");
                coworkerDTO.Id = coworker.Id;
                coworkerDTO.Name = coworker.Name;
                coworkerDTO.Birthday = coworker.Birthday.ToLocalTime();
                coworkerDTO.EMail = coworker.EMail;
                coworkerDTO.Position = coworker.Position;
                coworker.Password = coworker.Password;

                DateTimeOffset? validDateUpdate = null;
                if (!string.IsNullOrWhiteSpace(data[2]) &&
                    DateTimeOffset.TryParseExact(data[2], "dd.MM.yyyy", CultureInfo.InvariantCulture,
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

                Initializer.GetDbContext().Coworkers.Update(new Coworker
                {
                    Id = coworkerDTO.Id,
                    Name = coworkerDTO.Name,
                    Birthday = coworkerDTO.Birthday.ToUniversalTime(),
                    EMail = coworkerDTO.EMail,
                    Position = coworkerDTO.Position,
                    Password = coworkerDTO.Password
                });
                Initializer.GetDbContext().SaveChanges();

                success = true;
            }

            return success;
        }
        public static Table ShowAll(Category? category)
        {
            switch (category)
            {
                case Category.Coworkers:
                    return AllToTable(CoworkersToDto(Initializer.GetDbContext().Coworkers.ToList()));

                case Category.Projects:
                    return AllToTable(ProjectsToDto(Initializer.GetDbContext().Projects.ToList()));

                case Category.Tasks:
                    return AllToTable(TasksToDto(Initializer.GetDbContext().Tasks.ToList()));

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
            List<CoworkerDTO> coworkerDTOs = GetFilteredCoworkers(filters);
            var tableCoworkers = AllToTable(coworkerDTOs);
            return tableCoworkers;
        }
        public static Table ProjectsFiltered(List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            filters = DataToDictionary(filterOptions, filterOptionsValues);
            List<ProjectDTO> projectDTOs = GetFilteredProjects(filters);
            var tableProjects = AllToTable(projectDTOs);
            return tableProjects;
        }
        public static Table TasksFiltered(List<FilterOptions> filterOptions, List<string> filterOptionsValues)
        {
            Dictionary<FilterOptions, string> filters = new Dictionary<FilterOptions, string>();
            filters = DataToDictionary(filterOptions, filterOptionsValues);
            List<TaskDTO> taskDTOs = GetFilteredTasks(filters);
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

        private static List<CoworkerDTO> GetFilteredCoworkers(Dictionary<FilterOptions, string> filters)
        {
            var coworkers = Initializer.GetDbContext().Coworkers.ToList();
            var projects = Initializer.GetDbContext().Projects.ToList();
            var tasks = Initializer.GetDbContext().Tasks.ToList();

            List<Coworker> result = null;
            var coworkersByPosition = new List<Coworker>();
            var coworkersByProject = new List<Coworker>();

            foreach (var filter in filters)
            {
                switch (filter.Key) 
                {
                    case FilterOptions.Position:
                        coworkersByPosition = coworkers.Where(x => x.Position.ToString().ToLower() == filter.Value.ToLower()).ToList();
                        break;
                    case FilterOptions.Project:

                        var coworkersFromProjects = projects.Where(p => p.Name.ToLower() == filter.Value.ToLower())
                            .Select(x => x.Manager).ToList();
                        var coworkersFromTasks = tasks.Where(p => p.Project.Name.ToLower() == filter.Value.ToLower())
                            .Select(x => x.Employee).ToList();

                        coworkersByProject = new List<Coworker>();
                        coworkersByProject.AddRange(coworkersFromProjects);
                        coworkersByProject.AddRange(coworkersFromTasks);
                        coworkersByProject = coworkersByProject.Distinct().ToList();
                        break;
                }
            }

            if (filters.ContainsKey(FilterOptions.Position))
            {
                result = new List<Coworker>();
                result.AddRange(coworkersByPosition);
            }
            if (filters.ContainsKey(FilterOptions.Project))
            {
                if (result == null)
                {
                    result.AddRange(coworkersByProject);
                }
                else
                {
                    result = result.Intersect(coworkersByProject).ToList();
                }
            }

            return CoworkersToDto(result);

        }

        public static List<ProjectDTO> GetFilteredProjects(Dictionary<FilterOptions, string> filters)
        {
            var coworkers = Initializer.GetDbContext().Coworkers.ToList();
            var projects = Initializer.GetDbContext().Projects.ToList();
            var tasks = Initializer.GetDbContext().Tasks.ToList();

            List<Project> result = null;
            var projectByPriority = new List<Project>();
            var projectByCoworker = new List<Project>();

            foreach (var filter in filters)
            {
                switch (filter.Key)
                {
                    case FilterOptions.Priority:
                        projectByPriority = projects.Where(x => x.Priority.ToString().ToLower() == filter.Value.ToLower()).ToList();
                        break;
                    case FilterOptions.Coworker:

                        var projectsFromProjects = projects.Where(p => p.Manager.Name.ToLower() == filter.Value.ToLower()).ToList();
                        var projectsFromTasks = tasks.Where(p => p.Employee.Name.ToLower() == filter.Value.ToLower())
                            .Select(x => x.Project).ToList();

                        projectByCoworker = new List<Project>();
                        projectByCoworker.AddRange(projectsFromProjects);
                        projectByCoworker.AddRange(projectsFromTasks);
                        projectByCoworker = projectByCoworker.Distinct().ToList();
                        break;
                }
            }

            if (filters.ContainsKey(FilterOptions.Priority))
            {
                result = new List<Project>();
                result.AddRange(projectByPriority);
            }
            if (filters.ContainsKey(FilterOptions.Coworker))
            {
                if (result == null)
                {
                    result.AddRange(projectByCoworker);
                }
                else
                {
                    result = result.Intersect(projectByCoworker).ToList();
                }
            }

            return ProjectsToDto(result);

        }

        public static List<TaskDTO> GetFilteredTasks(Dictionary<FilterOptions, string> filters)
        {
            var tasks = Initializer.GetDbContext().Tasks.ToList();

            foreach (var filter in filters)
            {
                switch (filter.Key)
                {
                    case FilterOptions.Coworker:
                        tasks = tasks.Where(c => c.Manager.Name.ToLower() == filter.Value || c.Employee.Name.ToLower() == filter.Value).ToList();
                        break;
                    case FilterOptions.Priority:
                        tasks = tasks.Where(c => c.Priority.ToString().ToLower() == filter.Value).ToList();
                        break;
                    case FilterOptions.Project:
                        tasks = tasks.Where(c => c.Project.Name.ToLower() == filter.Value).ToList();
                        break;
                }
            }

            return TasksToDto(tasks);
        }


        private static List<CoworkerDTO> CoworkersToDto(List<Coworker> coworkers)
        {
            List<CoworkerDTO> list = new List<CoworkerDTO>();
            foreach (var coworker in coworkers)
            {
                list.Add(new CoworkerDTO()
                {
                    Id = coworker.Id,
                    Name = coworker.Name,
                    Birthday = coworker.Birthday,
                    EMail = coworker.EMail,
                    Position = coworker.Position
                });
            }

            return list;
        }

        private static List<ProjectDTO> ProjectsToDto(List<Project> projects)
        {
            List<ProjectDTO> list = new List<ProjectDTO>();
            foreach (var project in projects)
            {
                list.Add(new ProjectDTO()
                {
                    Id = project.Id,
                    Name = project.Name,
                    DueDate = project.DueDate,
                    Description = project.Description,
                    Priority = project.Priority,
                    ManagerId = project.ManagerId
                });
            }

            return list;
        } 
        
        private static List<TaskDTO> TasksToDto(List<Task> tasks)
        {
            List<TaskDTO> list = new List<TaskDTO>();
            foreach (var task in tasks)
            {
                list.Add(new TaskDTO()
                {
                    Id = task.Id,
                    Name = task.Name,
                    DueDate = task.DueDate,
                    Description = task.Description,
                    Priority = task.Priority,
                    ProjectId = task.ProjectId,
                    ManagerId = task.ManagerId,
                    EmployeeId = task.EmployeeId
                });
            }

            return list;
        }
    }
}
