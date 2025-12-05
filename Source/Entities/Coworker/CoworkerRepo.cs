using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace TaskTracking
{
    public class CoworkerRepo
    {
        private readonly string _connection;
        public CoworkerRepo(string connection)
        {
            _connection = connection;
        }
        public void DeleteCoworker(Coworker coworker)
        {
            string sql = "DELETE FROM users WHERE email = @email";
            using var npgsqlCommand = Connection(sql);
            npgsqlCommand.Parameters.AddWithValue("email", coworker.EMail);
            npgsqlCommand.ExecuteNonQuery();
        }
        public void UpdateCoworker(Coworker coworker)
        {
            string sql = "UPDATE users SET name = @n, birthday = @b, email = @e, position = @r, password_hash = @p WHERE id = @id ";
            using var npgsqlCommand = Connection(sql);
            CoworkerDataToSql(npgsqlCommand, coworker);
        }
        public void CreateCoworker(Coworker coworker)
        {
            string sql = "INSERT INTO users (name, birthday, email, position, password_hash) VALUES (@n, @b, @e, @r, @p)";
            using var npgsqlCommand = Connection(sql);
            CoworkerDataToSql(npgsqlCommand, coworker);
        }
        private void CoworkerDataToSql(NpgsqlCommand command, Coworker coworker)
        {
            command.Parameters.AddWithValue("id", coworker.Id);
            command.Parameters.AddWithValue("n", coworker.Name);
            command.Parameters.AddWithValue("b", coworker.Birthday);
            command.Parameters.AddWithValue("e", coworker.EMail);
            command.Parameters.AddWithValue("r", coworker.Position.ToString());
            command.Parameters.AddWithValue("p", coworker.Password);
            command.ExecuteNonQuery();
        }
        private NpgsqlCommand Connection(string sql)
        {
            var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            var command = new NpgsqlCommand(sql, connecting);
            return command;
        }
        public bool CheckIfUserExists(string mail)
        {
            string sql = "SELECT 1 FROM users WHERE email = @e";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("e", mail);
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
        public string GetPasswordHash(string mail)
        {
            string sql = "SELECT password_hash FROM users WHERE email = @e";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("e", mail);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }
        public Coworker? GetCoworkerByMail (string mail)
        {
            string sql = "SELECT id, name, birthday, email, position FROM users WHERE email = @e";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("e", mail);
            var gettingData = CoworkerData(command);
            return gettingData;
        }
        public Coworker? GetCoworkerById(int id)
        {
            string sql = "SELECT id, name, birthday, email, position FROM users WHERE id = @id";
            using var command = Connection(sql);
            command.Parameters.AddWithValue("id", id);
            var gettingData = CoworkerData(command);
            return gettingData;
        }

        private Coworker? CoworkerData(NpgsqlCommand command)
        {
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Coworker
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Birthday = reader.GetDateTime(2),
                    EMail = reader.GetString(3),
                    Position = Enum.Parse<Position>(reader.GetString(4), ignoreCase: true)
                };
            }
            return null;
        }

        private List<CoworkerDTO> AllCoworkersData(NpgsqlCommand command)
        {
            List<CoworkerDTO> list = new List<CoworkerDTO>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CoworkerDTO
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Birthday = reader.GetDateTime(2),
                    EMail = reader.GetString(3),
                    Position = Enum.Parse<Position>(reader.GetString(4), ignoreCase: true)
                });
            }
            return list;
        }

        public List<CoworkerDTO> GetAllCoworkers()
        {
            List<CoworkerDTO> list = new List<CoworkerDTO>();
            string sql = "SELECT * FROM users";
            using var command = Connection(sql);
            AllCoworkersData(command);
            return list;
        }

        public List<CoworkerDTO> GetFilteredCoworkers (Dictionary<FilterOptions, string> filters)
        {
            List<CoworkerDTO> coworkerDTOs = new List<CoworkerDTO>();
            List<string> foundFilters = new List<string>();
            List<NpgsqlParameter> foundValues = new List<NpgsqlParameter>();

            foreach (var f in filters)
            {
                if (f.Key == FilterOptions.Position)
                {
                    foundFilters.Add("LOWER(position) = LOWER(@position)");
                    foundValues.Add(new NpgsqlParameter("position", f.Value));
                }

                if (f.Key == FilterOptions.Project)
                {
                    foundFilters.Add("id IN (SELECT owner_id FROM projects WHERE id = @projId)" + " OR id IN (SELECT coworker_id FROM tasks WHERE project_id = @projId)");
                    foundValues.Add(new NpgsqlParameter("projId", int.Parse(f.Value)));
                }
            }

            string sql = "SELECT * FROM users" + " WHERE " + string.Join(" AND ", foundFilters);
            using var command = Connection(sql);
            command.Parameters.AddRange(foundValues.ToArray());
            coworkerDTOs = AllCoworkersData(command);
            return coworkerDTOs;
        }
    }
}
