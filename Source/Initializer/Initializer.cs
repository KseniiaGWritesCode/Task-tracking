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

        public static void ConnectToDB()
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
        }

        public static CoworkerRepo GetCoworkerRepo()
        {
            if (_coworkerRepo == null)
            {
                _coworkerRepo = new CoworkerRepo(_npgsqlConnection);
            }

            return _coworkerRepo;
        }

        public static ProjectRepo GetProjectRepo()
        {
            if (_projectRepo == null)
            {
                _projectRepo = new ProjectRepo(_npgsqlConnection);
            }

            return _projectRepo;
        }

        public static TaskRepo GetTaskRepo()
        {
            if (_taskRepo == null)
            {
                _taskRepo = new TaskRepo(_npgsqlConnection);
            }

            return _taskRepo;
        }

    }
}
