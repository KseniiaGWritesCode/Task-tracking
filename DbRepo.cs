using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class DbRepo
    {
        private string _connection;

        // TODO one connection for whole runtime --> save 300-500ms per request

        public DbRepo (string connection)
        {
            _connection = connection;




        }

        protected NpgsqlCommand Connection(string sql)
        {
            var connecting = new NpgsqlConnection(_connection);
            connecting.Open();
            var command = new NpgsqlCommand(sql, connecting);
            //connecting.Close();
            return command;
        }
    }
}
