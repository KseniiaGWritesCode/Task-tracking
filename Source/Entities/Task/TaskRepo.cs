using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaskTracking
{
    public class TaskRepo
    {
        private static string _connection = "Host=localhost;Port=5432;Database=TaskTracking;Username=postgres;Password=ILove6Bo0bs;Include Error Detail=true;";
        private readonly CoworkerRepo coworkerrepo = new CoworkerRepo(_connection);
        private readonly ProjectRepo projectrepo = new ProjectRepo(_connection);

        public TaskRepo(string connection)
        {
            _connection = connection;
        }

        public void DeleteTask(Task task)
        {
            string sql = "DELETE FROM tasks WHERE id=@id";
            var npgsqlCommand = Connection(sql);
            npgsqlCommand.Parameters.AddWithValue("id", task.Id);
            npgsqlCommand.ExecuteNonQuery();
        }

        public void UpdateTask(Task task)
        {
            string sql = "UPDATE tasks SET name = @name, due_date = @date, description = @descr, priority = @prio, project_id = @project, manager_id = @manager, coworker_id = @coworker WHERE id = @id";
            var npgsqlCommand = Connection(sql);
            TaskDataToSql(npgsqlCommand, task);
        }

        public void CreateTask(Task task)
        {
            string sql = "INSERT INTO tasks (name, due_date, description, priority, project_id, manager_id, coworker_id) VALUES (@name, @date, @descr, @prio, @project, @manager, @coworker)";
            using var npgsqlCommand = Connection(sql);
            TaskDataToSql(npgsqlCommand, task);
        }

        private NpgsqlCommand Connection(string sql)
        {
            var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            var command = new NpgsqlCommand(sql, connecting);
            return command;
        }

        private void TaskDataToSql(NpgsqlCommand command, Task task)
        {
            command.Parameters.AddWithValue("id", task.Id);
            command.Parameters.AddWithValue("name", task.Name);
            command.Parameters.AddWithValue("date", task.DueDate);
            command.Parameters.AddWithValue("descr", task.Description);
            command.Parameters.AddWithValue("prio", task.Priority.ToString());
            command.Parameters.AddWithValue("project", task.ProjectId);
            command.Parameters.AddWithValue("manager", task.ManagerId);
            command.Parameters.AddWithValue("coworker", task.EmployeeId);
            command.ExecuteNonQuery();
        }

        public Task? GetTaskById(int id)
        {
            string sql = "SELECT id, name, due_date, description, priority, project_id, manager_id, coworker_id FROM tasks WHERE id = @id";
            var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            var gettingData = TaskData(command);
            return gettingData;
        }

        public bool CheckIfTaskExists(int id)
        {
            string sql = "SELECT 1 FROM tasks WHERE id = @id";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Task? TaskData (NpgsqlCommand command)
        {
            int projectId = 0;
            int managerId = 0;
            int coworkerId = 0;
            var task = new Task();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                task = new Task()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DueDate = reader.GetDateTime(2),
                    Description = reader.GetString(3),
                    Priority = Enum.Parse<Priority>(reader.GetString(4)),
                };
                projectId = reader.GetInt32(5);
                managerId = reader.GetInt32(6);
                coworkerId = reader.GetInt32(7);
                var project = projectrepo.GetProjectById(projectId);
                task.ProjectId = project.Id;
                var manager = coworkerrepo.GetCoworkerById(managerId);
                task.ManagerId = manager.Id;
                var coworker = coworkerrepo.GetCoworkerById(coworkerId);
                task.EmployeeId = coworker.Id;
            }
            return task;
        }
        private List<TaskDTO> AllTasksData(NpgsqlCommand command)
        {
            List<TaskDTO> taskDTOs = new List<TaskDTO>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                taskDTOs.Add(new TaskDTO()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DueDate = reader.GetDateTime(2),
                    Description = reader.GetString(3),
                    Priority = Enum.Parse<Priority>(reader.GetString(4), ignoreCase: true),
                    ProjectId = reader.GetInt32(5),
                    ManagerId = reader.GetInt32(6),
                    EmployeeId = reader.GetInt32(7)
                });
            }
            return taskDTOs;
        }
        public List<TaskDTO> GetAllTasks()
        {
            List<TaskDTO> list = new List<TaskDTO>();
            string sql = "SELECT * FROM tasks";
            using var command = Connection(sql);
            list = AllTasksData(command);
            return list;
        }
        public List<TaskDTO> GetFilteredTasks(Dictionary<FilterOptions, string> filters)
        {
            List<TaskDTO> list = new List<TaskDTO>();
            List<string> foundFilters = new List<string>();
            List<NpgsqlParameter> foundValues = new List<NpgsqlParameter>();

            foreach (var f in filters)
            {
                if (f.Key == FilterOptions.Coworker)
                {
                    foundFilters.Add("(manager_id = @coworkerId OR coworker_id = @coworkerId)");
                    foundValues.Add(new NpgsqlParameter("coworkerId", int.Parse(f.Value)));
                }
                if (f.Key == FilterOptions.Priority)
                {
                    foundFilters.Add("LOWER(priority) = LOWER(@priority)");
                    foundValues.Add(new NpgsqlParameter("priority", f.Value));
                }
                if (f.Key == FilterOptions.Project)
                {
                    foundFilters.Add("project_id = @projectid");
                    foundValues.Add(new NpgsqlParameter("projectid", f.Value));
                }
            }
            string sql = "SELECT * FROM tasks" + " WHERE " + string.Join(" AND ", foundFilters);
            using var command = Connection(sql);
            command.Parameters.AddRange(foundValues.ToArray());
            list = AllTasksData(command);
            return list;
        }
    }
}
