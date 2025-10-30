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
        private readonly string _connection;
        CoworkerRepo coworkerrepo;
        ProjectRepo projectrepo;

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
            var npgsqlCommand = Connection(sql);
            TaskDataToSql(npgsqlCommand, task);
        }

        private NpgsqlCommand Connection(string sql)
        {
            using var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            using var command = new NpgsqlCommand(sql, connecting);
            return command;
        }

        private void TaskDataToSql(NpgsqlCommand command, Task task)
        {
            command.Parameters.AddWithValue("name", task.Name);
            command.Parameters.AddWithValue("date", task.DueDate);
            command.Parameters.AddWithValue("descr", task.Description);
            command.Parameters.AddWithValue("prio", task.Priority.ToString());
            command.Parameters.AddWithValue("project", task.Project.Id);
            command.Parameters.AddWithValue("manager", task.Manager.Id);
            command.Parameters.AddWithValue("coworker", task.Employee.Id);
            command.ExecuteNonQuery();
        }

        public Task? GetTaskById(int id, string sql)
        {
            sql = "SELECT id, name, due_date, description, priority, project_id, manager_id, coworker_id, created_by, created_at, updated_at FROM tasks WHERE id = @id";
            var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            using var reader = command.ExecuteReader();
            int projectId = 0;
            int managerId = 0;
            int coworkerId = 0;
            int createdBy = 0;
            var task = new Task(); 

            if(reader.Read())
            {
                task = new Task()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DueDate = reader.GetDateTime(2),
                    Description = reader.GetString(3),
                    Priority = Enum.Parse<Priority>(reader.GetString(4)),
                    CreatedAt = reader.GetDateTime(9),
                    UpdatedAt = reader.GetDateTime(10)
                };
                projectId = reader.GetInt32(5);
                managerId = reader.GetInt32(6);
                coworkerId = reader.GetInt32(7);
                createdBy = reader.GetInt32(8);
                var project = projectrepo.GetProjectById(projectId, sql);
                task.Project = project;
                var manager = coworkerrepo.GetCoworkerById(managerId, sql);
                task.Manager = manager;
                var coworker = coworkerrepo.GetCoworkerById(coworkerId, sql);
                task.Employee = coworker;
                var whoCreated = coworkerrepo.GetCoworkerById(createdBy, sql);
                task.CreatedBy = whoCreated;
            }
            return task;
        }
    }
}
