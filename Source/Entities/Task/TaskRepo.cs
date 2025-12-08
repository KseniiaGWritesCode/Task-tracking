//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace TaskTracking
//{
//    public class TaskRepo : DbRepo
//    {
//        public TaskRepo(NpgsqlConnection connection) : base(connection) { }

//        public void DeleteTask(Task task)
//        {
//            string sql = "DELETE FROM tasks WHERE id=@id";
//            Command(sql).AddWithValue("id", task.Id).ExecuteNonQuery();
//        }

//        public void UpdateTask(Task task)
//        {
//            string sql = "UPDATE tasks SET name = @name, due_date = @date, description = @descr, priority = @prio, project_id = @project, manager_id = @manager, coworker_id = @coworker WHERE id = @id";
//            TaskDataToSql(sql, task);
//        }

//        public void CreateTask(Task task)
//        {
//            string sql = "INSERT INTO tasks (name, due_date, description, priority, project_id, manager_id, coworker_id) VALUES (@name, @date, @descr, @prio, @project, @manager, @coworker)";
//            TaskDataToSql(sql, task);
//        }

//        private void TaskDataToSql(string sql,  Task task)
//        {
//            Command(sql)
//                .AddWithValue("id", task.Id)
//                .AddWithValue("name", task.Name)
//                .AddWithValue("date", task.DueDate)
//                .AddWithValue("descr", task.Description)
//                .AddWithValue("prio", task.Priority.ToString())
//                .AddWithValue("project", task.ProjectId)
//                .AddWithValue("manager", task.ManagerId)
//                .AddWithValue("coworker", task.EmployeeId)
//                .ExecuteNonQuery();
//        }

//        public Task? GetTaskById(int id)
//        {
//            string sql = "SELECT id, name, due_date, description, priority, project_id, manager_id, coworker_id FROM tasks WHERE id = @id";
//            var reader = Command(sql).AddWithValue("id", id).ExecuteReader();
//            return TaskData(reader);
//        }

//        public bool CheckIfTaskExists(int id)
//        {
//            string sql = "SELECT 1 FROM tasks WHERE id = @id";
//            return Command(sql).AddWithValue("id", id).CheckIfEntityExists();
//        }
//        private Task? TaskData (NpgsqlDataReader reader)
//        {
//            int projectId = 0;
//            int managerId = 0;
//            int coworkerId = 0;
//            Task task = null;

//            if (reader.Read())
//            {
//                task = new Task()
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    DueDate = reader.GetDateTime(2),
//                    Description = reader.GetString(3),
//                    Priority = Enum.Parse<Priority>(reader.GetString(4)),
//                };
//                projectId = reader.GetInt32(5);
//                managerId = reader.GetInt32(6);
//                coworkerId = reader.GetInt32(7);
//                var project = Initializer.GetProjectRepo().GetProjectById(projectId);
//                task.ProjectId = project.Id;
//                var manager = Initializer.GetCoworkerRepo().GetCoworkerById(managerId);
//                task.ManagerId = manager.Id;
//                var coworker = Initializer.GetCoworkerRepo().GetCoworkerById(coworkerId);
//                task.EmployeeId = coworker.Id;
//            }
//            reader.Close();
//            return task;
//        }
//        private List<TaskDTO> AllTasksData(NpgsqlDataReader reader)
//        {
//            List<TaskDTO> taskDTOs = new List<TaskDTO>();
//            while (reader.Read())
//            {
//                taskDTOs.Add(new TaskDTO()
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    DueDate = reader.GetDateTime(2),
//                    Description = reader.GetString(3),
//                    Priority = Enum.Parse<Priority>(reader.GetString(4), ignoreCase: true),
//                    ProjectId = reader.GetInt32(5),
//                    ManagerId = reader.GetInt32(6),
//                    EmployeeId = reader.GetInt32(7)
//                });
//            }
//            reader.Close();
//            return taskDTOs;
//        }
//        public List<TaskDTO> GetAllTasks()
//        {
//            string sql = "SELECT * FROM tasks";
//            return AllTasksData(Command(sql).ExecuteReader());
//        }
//        public List<TaskDTO> GetFilteredTasks(Dictionary<FilterOptions, string> filters)
//        {
//            List<string> foundFilters = new List<string>();
//            List<NpgsqlParameter> foundValues = new List<NpgsqlParameter>();

//            foreach (var f in filters)
//            {
//                if (f.Key == FilterOptions.Coworker)
//                {
//                    foundFilters.Add("(manager_id = @coworkerId OR coworker_id = @coworkerId)");
//                    foundValues.Add(new NpgsqlParameter("coworkerId", int.Parse(f.Value)));
//                }
//                if (f.Key == FilterOptions.Priority)
//                {
//                    foundFilters.Add("LOWER(priority) = LOWER(@priority)");
//                    foundValues.Add(new NpgsqlParameter("priority", f.Value));
//                }
//                if (f.Key == FilterOptions.Project)
//                {
//                    foundFilters.Add("project_id = @projectid");
//                    foundValues.Add(new NpgsqlParameter("projectid", f.Value));
//                }
//            }
//            string sql = "SELECT * FROM tasks" + " WHERE " + string.Join(" AND ", foundFilters);
//            var reader = Command(sql).AddRange(foundValues.ToArray()).ExecuteReader();
//            return AllTasksData(reader);
//        }
//    }
//}
