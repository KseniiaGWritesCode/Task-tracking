using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ProjectRepo
    {
        private readonly string _connection;
        CoworkerRepo coworkerrepo;
        public ProjectRepo(string connection)
        {
            _connection = connection;
        }
        public void DeleteProject(Project project)
        {
            string sql = "DELETE FROM projects WHERE id=@id";
            var npgsqlCommand = Connection(sql);
            npgsqlCommand.Parameters.AddWithValue("id", project.Id);
            npgsqlCommand.ExecuteNonQuery();
        }
        public void UpdateProject(Project project)
        {
            string sql = "UPDATE projects SET name = @name, due_date = @date, description = @descr, priority = @prio, owner_id = @manager WHERE id = @id";
            var npgsqlCommand = Connection(sql);
            ProjectDataToSql(npgsqlCommand, project);
        }
        public void CreateProject(Project project)
        {
            string sql = "INSERT INTO projects (name, due_date, description, priority, owner_id) VALUES (@name, @date, @descr, @prio, @manager)";
            var npgsqlCommand = Connection(sql);
            ProjectDataToSql(npgsqlCommand, project);
        }
        private void ProjectDataToSql(NpgsqlCommand command, Project project)
        {
            command.Parameters.AddWithValue("name",project.Name);
            command.Parameters.AddWithValue("date", project.DueDate);
            command.Parameters.AddWithValue("descr", project.Description);
            command.Parameters.AddWithValue("prio", project.Priority.ToString());
            command.Parameters.AddWithValue("manager", project.Manager.Id);
            command.ExecuteNonQuery();
        }
        private NpgsqlCommand Connection(string sql)
        {
            using var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            using var command = new NpgsqlCommand(sql, connecting);
            return command;
        }
        public Project? GetProjectByName(string name, string sql)
        {
            sql = "SELECT id, name, due_date, description, priority, owner_id, created_by, created_at, updated_at FROM projects WHERE name = @name";
            var command = Connection(sql);
            command.Parameters.AddWithValue("name", name);
            var gettingData = ProjectData(command, sql);
            return gettingData;
        }

        public Project? GetProjectById (int id, string sql)
        {
            sql = "SELECT id, name, due_date, description, priority, owner_id, created_by, created_at, updated_at FROM projects WHERE id = @id";
            var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            var gettingData = ProjectData(command, sql);
            return gettingData;
        }

        private Project? ProjectData(NpgsqlCommand command, string sql)
        {
            int managerId = 0;
            int createdBy = 0;
            var project = new Project();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                project = new Project()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DueDate = reader.GetDateTime(2),
                    Description = reader.GetString(3),
                    Priority = Enum.Parse<Priority>(reader.GetString(4)),
                    CreatedAt = reader.GetDateTime(7),
                    UpdatedAt = reader.GetDateTime(8)
                };
                managerId = reader.GetInt32(5);
                createdBy = reader.GetInt32(6);
                var manager = coworkerrepo.GetCoworkerById(managerId);
                project.Manager = manager;
                var whoCreated = coworkerrepo.GetCoworkerById(createdBy);
                project.CreatedBy = whoCreated;
            }
            return project;
        }
    }
}
