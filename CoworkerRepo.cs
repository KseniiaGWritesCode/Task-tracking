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
            string sql = "DELETE FROM users WHERE id=@id";
            var npgsqlCommand = Connection(sql);
            npgsqlCommand.Parameters.AddWithValue("id", coworker.Id);
            npgsqlCommand.ExecuteNonQuery();
        }
        public void UpdateCoworker(Coworker coworker)
        {
            string sql = "UPDATE users SET name = @n, birthday = @b, email = @e, position = @r, password_hash = @p WHERE id = @id ";
            var npgsqlCommand = Connection(sql);
            CoworkerDataToSql(npgsqlCommand, coworker);
        }
        public void CreateCoworker(Coworker coworker)
        {
            string sql = "INSERT INTO users (name, birthday, email, position, password_hash) VALUES (@n, @b, @e, @r, @p)";
            var npgsqlCommand = Connection(sql);
            CoworkerDataToSql(npgsqlCommand, coworker);
        }
        private void CoworkerDataToSql(NpgsqlCommand command, Coworker coworker)
        {
            command.Parameters.AddWithValue("n", coworker.Name);
            command.Parameters.AddWithValue("b", coworker.Birthday);
            command.Parameters.AddWithValue("e", coworker.EMail);
            command.Parameters.AddWithValue("r", coworker.Position.ToString());
            command.Parameters.AddWithValue("p", coworker.Password);
            command.ExecuteNonQuery();
        }
        private NpgsqlCommand Connection(string sql)
        {
            using var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            using var command = new NpgsqlCommand(sql, connecting);
            return command;
        }
        public Coworker? GetCoworker(string mail, string sql)
        {
            sql = "SELECT id, name, birthday, email, position, password_hash FROM users WHERE email = @e";
            var command = Connection(sql);
            command.Parameters.AddWithValue("e", mail);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Coworker
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Birthday = reader.GetDateTime(2),
                    EMail = reader.GetString(3),
                    Position = Enum.Parse<Position>(reader.GetString(4)),
                    Password = reader.GetString(5)
                };
            }
            return null;
        }
    }
}
