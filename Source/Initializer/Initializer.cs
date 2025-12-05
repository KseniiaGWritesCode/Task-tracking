using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public static class Initializer
    {
        private static AppConfig _appConfig;
        private static NpgsqlConnection _npgsqlConnection;
        private static CoworkerRepo _coworkerRepo;
        private static ProjectRepo _projectRepo;
        private static TaskRepo _taskRepo;

        public static void LoadConfig(string fileName)
        {
            _appConfig = new AppConfig(fileName);
        }

        public static NpgsqlConnection GetConnectionToDB()
        {
            if (_appConfig == null)
            {
                throw new InvalidOperationException("Call LoadConfig first!");
            }

            if (_npgsqlConnection == null)
            {
                _npgsqlConnection = new NpgsqlConnection(_appConfig.GetConnectionString());
                _npgsqlConnection.Open();
            }

            return _npgsqlConnection;
        }

        public static CoworkerRepo GetCoworkerRepo(NpgsqlConnection connection)
        {
            if (_coworkerRepo == null)
            {
                _coworkerRepo = new CoworkerRepo(connection);
            }

            return _coworkerRepo;
        }

        public static ProjectRepo GetProjectRepo(NpgsqlConnection connection)
        {
            if (_projectRepo == null)
            {
                _projectRepo = new ProjectRepo(connection);
            }

            return _projectRepo;
        }

        public static TaskRepo GetTaskRepo(NpgsqlConnection connection)
        {
            if (_taskRepo == null)
            {
                _taskRepo = new TaskRepo(connection);
            }

            return _taskRepo;
        }

    }
}
