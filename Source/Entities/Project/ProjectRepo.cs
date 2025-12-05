using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaskTracking
{
    public class ProjectRepo : DbRepo
    {
        public ProjectRepo(NpgsqlConnection connection) : base(connection) { }

        public void DeleteProject(Project project)
        {
            string sql = "DELETE FROM projects WHERE id=@id";
            Command(sql).AddWithValue("id", project.Id).ExecuteNonQuery();
        }
        public void UpdateProject(Project project)
        {
            string sql = "UPDATE projects SET name = @name, due_date = @date, description = @descr, priority = @prio, owner_id = @manager WHERE id = @id";
            ProjectDataToSql(sql, project);
        }
        public void CreateProject(Project project)
        {
            string sql = "INSERT INTO projects (name, due_date, description, priority, owner_id) VALUES (@name, @date, @descr, @prio, @manager)";
            ProjectDataToSql(sql, project);
        }
        private void ProjectDataToSql(string sql, Project project)
        {
            Command(sql)
                .AddWithValue("id", project.Id)
                .AddWithValue("name", project.Name)
                .AddWithValue("date", project.DueDate)
                .AddWithValue("descr", project.Description)
                .AddWithValue("prio", project.Priority.ToString())
                .AddWithValue("manager", project.ManagerId)
                .ExecuteNonQuery();
        }

        public Project? GetProjectByName(string name)
        {
            string sql = "SELECT id, name, due_date, description, priority, owner_id FROM projects WHERE name = @name";
            var reader = Command(sql).AddWithValue("name", name).ExecuteReader();
            return ProjectData(reader);
        }
        public Project? GetProjectById (int id)
        {
            string sql = "SELECT id, name, due_date, description, priority, owner_id FROM projects WHERE id = @id";
            var reader = Command(sql).AddWithValue("id", id).ExecuteReader();
            return ProjectData(reader);
        }
        public bool CheckIfProjectExists(int id)
        {
            string sql = "SELECT 1 FROM projects WHERE id = @id";
            return Command(sql).AddWithValue("id", id).CheckIfEntityExists();
        }
        private Project? ProjectData(NpgsqlDataReader reader)
        {
            int managerId = 0;
            int createdBy = 0;
            Project project = null;
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
                var manager = Initializer.GetCoworkerRepo().GetCoworkerById(managerId);
                project.ManagerId = manager.Id;
            }
            reader.Close();
            return project;
        }
        private List<ProjectDTO> AllProjectsData(NpgsqlDataReader reader)
        {
            List<ProjectDTO> list = new List<ProjectDTO>();
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
            reader.Close();
            return list;
        }
        public List<ProjectDTO> GetAllProjects()
        {
            string sql = "SELECT * FROM projects";
            return AllProjectsData(Command(sql).ExecuteReader());
        }
        public List<ProjectDTO> GetFilteredProjects(Dictionary<FilterOptions, string> filters)
        {
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
            var reader = Command(sql).AddRange(foundValues.ToArray()).ExecuteReader();
            return AllProjectsData(reader);
        }
    }
}
