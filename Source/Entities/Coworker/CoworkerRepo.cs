//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata;
//using System.Text;
//using System.Threading.Tasks;
//using Npgsql;

//namespace TaskTracking
//{
//    public class CoworkerRepo //: DbRepo
//    {
//        //public CoworkerRepo(NpgsqlConnection connection) : base (connection) { }

//        //public void DeleteCoworker(Coworker coworker)
//        //{
//        //    string sql = "DELETE FROM users WHERE email = @email";
//        //    Command(sql).AddWithValue("email", coworker.EMail).ExecuteNonQuery();
//        //}
//        //public void UpdateCoworker(Coworker coworker)
//        //{
//        //    string sql = "UPDATE users SET name = @n, birthday = @b, email = @e, position = @r, password_hash = @p WHERE id = @id ";
//        //    CoworkerDataToSql(sql, coworker);
//        //}
//        //public void CreateCoworker(Coworker coworker)
//        //{
//        //    string sql = "INSERT INTO users (name, birthday, email, position, password_hash) VALUES (@n, @b, @e, @r, @p)";
//        //    CoworkerDataToSql(sql, coworker);
//        //}
//        //private void CoworkerDataToSql(string sql, Coworker coworker)
//        //{
//        //    Command(sql)
//        //        .AddWithValue("email", coworker.EMail)    
//        //        .AddWithValue("id", coworker.Id)
//        //        .AddWithValue("n", coworker.Name)
//        //        .AddWithValue("b", coworker.Birthday)
//        //        .AddWithValue("e", coworker.EMail)
//        //        .AddWithValue("r", coworker.Position.ToString())
//        //        .AddWithValue("p", coworker.Password)
//        //        .ExecuteNonQuery();
//        //}
//        public bool CheckIfUserExists(string mail)
//        {
//            string sql = "SELECT 1 FROM users WHERE email = @e";
//            return Command(sql).AddWithValue("e", mail).CheckIfEntityExists();
//        }
//        public string GetPasswordHash(string mail)
//        {
//            string sql = "SELECT password_hash FROM users WHERE email = @e";
//            return Command(sql).AddWithValue("e", mail).ExecuteScalar()?.ToString();
//        }
//        public Coworker? GetCoworkerByMail (string mail)
//        {
//            string sql = "SELECT id, name, birthday, email, position FROM users WHERE email = @e";
//            var reader = Command(sql).AddWithValue("e", mail).ExecuteReader();
//            return CoworkerData(reader);
//        }
//        public Coworker? GetCoworkerById(int id)
//        {
//            string sql = "SELECT id, name, birthday, email, position FROM users WHERE id = @id";
//            var reader = Command(sql).AddWithValue("id", id).ExecuteReader();
//            return CoworkerData(reader);
//        }

//        private Coworker? CoworkerData(NpgsqlDataReader reader)
//        {
//            Coworker result = null;
//            if (reader.Read())
//            {
//                result = new Coworker
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    Birthday = reader.GetDateTime(2),
//                    EMail = reader.GetString(3),
//                    Position = Enum.Parse<Position>(reader.GetString(4), ignoreCase: true)
//                };
//            }

//            reader.Close();
//            return result;
//        }

//        private List<CoworkerDTO> AllCoworkersData(NpgsqlDataReader reader)
//        {
//            List<CoworkerDTO> list = new List<CoworkerDTO>();
//            while (reader.Read())
//            {
//                list.Add(new CoworkerDTO
//                {
//                    Id = reader.GetInt32(0),
//                    Name = reader.GetString(1),
//                    Birthday = reader.GetDateTime(2),
//                    EMail = reader.GetString(3),
//                    Position = Enum.Parse<Position>(reader.GetString(4), ignoreCase: true)
//                });
//            }
//            reader.Close();
//            return list;
//        }

//        public List<CoworkerDTO> GetAllCoworkers()
//        {
//            string sql = "SELECT * FROM users";
//            return AllCoworkersData(Command(sql).ExecuteReader());
//        }

//        public List<CoworkerDTO> GetFilteredCoworkers (Dictionary<FilterOptions, string> filters)
//        {
//            List<string> foundFilters = new List<string>();
//            List<NpgsqlParameter> foundValues = new List<NpgsqlParameter>();

//            foreach (var f in filters)
//            {
//                if (f.Key == FilterOptions.Position)
//                {
//                    foundFilters.Add("LOWER(position) = LOWER(@position)");
//                    foundValues.Add(new NpgsqlParameter("position", f.Value));
//                }

//                if (f.Key == FilterOptions.Project)
//                {
//                    foundFilters.Add("id IN (SELECT owner_id FROM projects WHERE id = @projId)" + " OR id IN (SELECT coworker_id FROM tasks WHERE project_id = @projId)");
//                    foundValues.Add(new NpgsqlParameter("projId", int.Parse(f.Value)));
//                }
//            }

//            string sql = "SELECT * FROM users" + " WHERE " + string.Join(" AND ", foundFilters);
//            var reader = Command(sql).AddRange(foundValues.ToArray()).ExecuteReader();
//            return AllCoworkersData(reader);
//        }
//    }
//}
