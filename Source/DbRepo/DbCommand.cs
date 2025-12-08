//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Npgsql;

//namespace TaskTracking
//{
//    public class DbCommand
//    {
//        private NpgsqlCommand _command;
//        private NpgsqlConnection _connection;

//        public DbCommand(NpgsqlConnection connection, string sql)
//        {
//            _connection = connection;
//            _command = new NpgsqlCommand(sql, _connection);
//        }

//        public DbCommand AddWithValue(string paramName, object value)
//        {
//            _command.Parameters.AddWithValue(paramName, value);
//            return this;
//        }
//        public DbCommand AddRange(Array values)
//        {
//            _command.Parameters.AddRange(values);
//            return this;
//        }

//        public int ExecuteNonQuery()
//        {
//            int result = _command.ExecuteNonQuery();
//            _command.Dispose();
//            return result;
//        }

//        public bool CheckIfEntityExists()
//        {
//            using var reader = _command.ExecuteReader();
//            _command.Dispose();
//            return reader.Read();
//        }

//        public NpgsqlDataReader ExecuteReader()
//        {
//            var result = _command.ExecuteReader();
//            _command.Dispose();
//            return result;
//        }

//        public object? ExecuteScalar()
//        {
//            var result = _command.ExecuteScalar();
//            _command.Dispose();
//            return result;
//        }
        
//        ~DbCommand()
//        {
//            _command?.Dispose();
//        }
//    }
//}
