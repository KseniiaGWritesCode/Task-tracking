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
        private static string _connection = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=ILove6Bo0bs;Include Error Detail=true;";
        private readonly CoworkerRepo coworkerrepo = new CoworkerRepo(_connection);
        public ProjectRepo(string connection)
        {
            _connection = connection;
        }
        public void DeleteProject(Project project)
        {
            string sql = "DELETE FROM projects WHERE id=@id";
            using var npgsqlCommand = Connection(sql);
            npgsqlCommand.Parameters.AddWithValue("id", project.Id);
            npgsqlCommand.ExecuteNonQuery();
        }
        public void UpdateProject(Project project)
        {
            string sql = "UPDATE projects SET name = @name, due_date = @date, description = @descr, priority = @prio, owner_id = @manager WHERE id = @id";
            using var npgsqlCommand = Connection(sql);
            ProjectDataToSql(npgsqlCommand, project);
        }
        public void CreateProject(Project project)
        {
            string sql = "INSERT INTO projects (name, due_date, description, priority, owner_id) VALUES (@name, @date, @descr, @prio, @manager)";
            using var npgsqlCommand = Connection(sql);
            ProjectDataToSql(npgsqlCommand, project);
        }
        private void ProjectDataToSql(NpgsqlCommand command, Project project)
        {
            command.Parameters.AddWithValue("id", project.Id);
            command.Parameters.AddWithValue("name",project.Name);
            command.Parameters.AddWithValue("date", project.DueDate);
            command.Parameters.AddWithValue("descr", project.Description);
            command.Parameters.AddWithValue("prio", project.Priority.ToString());
            command.Parameters.AddWithValue("manager", project.ManagerId);
            command.ExecuteNonQuery();
        }
        private NpgsqlCommand Connection(string sql)
        {
            var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            var command = new NpgsqlCommand(sql, connecting);
            return command;
        }
        public Project? GetProjectByName(string name)
        {
            string sql = "SELECT id, name, due_date, description, priority, owner_id FROM projects WHERE name = @name";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("name", name);
            var gettingData = ProjectData(command);
            return gettingData;
        }
        public Project? GetProjectById (int id)
        {
            string sql = "SELECT id, name, due_date, description, priority, owner_id FROM projects WHERE id = @id";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            var gettingData = ProjectData(command);
            return gettingData;
        }
        public bool CheckIfProjectExists(int id)
        {
            string sql = "SELECT 1 FROM projects WHERE id = @id";
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
        private Project? ProjectData(NpgsqlCommand command)
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
                };
                managerId = reader.GetInt32(5);
                var manager = coworkerrepo.GetCoworkerById(managerId);
                project.ManagerId = manager.Id;
            }
            return project;
        }
        private List<ProjectDTO> AllProjectsData(NpgsqlCommand command)
        {
            List<ProjectDTO> list = new List<ProjectDTO>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ProjectDTO()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DueDate = reader.GetDateTime(2),
                    Description = reader.GetString(3),
                    Priority = Enum.Parse<Priority>(reader.GetString(4), ignoreCase: true),
                    ManagerId = reader.GetInt32(5)
                });
            }
            return list;
        }
        public List<ProjectDTO> GetAllProjects()
        {
            List<ProjectDTO> list = new List<ProjectDTO>();
            string sql = "SELECT * FROM projects";
            using var command = Connection(sql);
            list = AllProjectsData(command);
            return list;
        }
        public List<ProjectDTO> GetFilteredProjects(Dictionary<FilterOptions, string> filters)
        {
            List<ProjectDTO> list = new List<ProjectDTO>();
            List<string> foundFilters = new List<string>();
            List<NpgsqlParameter> foundValues = new List<NpgsqlParameter>();

            foreach (var f in filters)
            {
                if (f.Key == FilterOptions.Coworker)
                {
                    foundFilters.Add("owner_id = @coworkerId" + " OR id IN (SELECT project_id FROM tasks WHERE coworker_id = @coworkerId) " + " OR id IN (SELECT project_id FROM tasks WHERE manager_id = @coworkerId)");
                    foundValues.Add(new NpgsqlParameter("coworkerId", int.Parse(f.Value)));
                }
                if (f.Key == FilterOptions.Priority)
                {
                    foundFilters.Add("LOWER(priority) = LOWER(@priority)");
                    foundValues.Add(new NpgsqlParameter("priority", f.Value));
                }
            }

            string sql = "SELECT * FROM projects" + " WHERE " + string.Join(" AND ", foundFilters);
            using var command = Connection(sql);
            command.Parameters.AddRange(foundValues.ToArray());
            list = AllProjectsData(command);
            return list;
        }
    }
}
