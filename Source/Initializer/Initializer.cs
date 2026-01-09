using Microsoft.EntityFrameworkCore;
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
        public const string APP_CONFIG_FILE = "config.json";

        private static AppConfig _appConfig;
        //private static NpgsqlConnection _npgsqlConnection;
        //private static CoworkerRepo _coworkerRepo;
        //private static ProjectRepo _projectRepo;
        //private static TaskRepo _taskRepo;
        private static AppDbContext _appDbContext;

        //public static void LoadConfig()
        //{
        //    _appConfig = new AppConfig(APP_CONFIG_FILE);
        //}

        public static void InitEntities()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(Initializer.GetEarlyConnectionString())
            .Options;

            _appDbContext = new AppDbContext(options);

            //MigrationValidator.ValidateModelAndMigrations(_appDbContext);

            _appDbContext.Database.Migrate();
        }

        public static AppDbContext GetDbContext() => _appDbContext;

        public static string GetEarlyConnectionString()
        {
            _appConfig = new AppConfig(APP_CONFIG_FILE);
            return _appConfig.GetConnectionString();
        }

        //public static void ConnectToDB()
        //{
        //    if (_appConfig == null)
        //    {
        //        throw new InvalidOperationException("Call LoadConfig first!");
        //    }

        //    if (_npgsqlConnection == null)
        //    {
        //        _npgsqlConnection = new NpgsqlConnection(_appConfig.GetConnectionString());
        //        _npgsqlConnection.Open();
        //    }
        //}

        //public static CoworkerRepo GetCoworkerRepo()
        //{
        //    if (_coworkerRepo == null)
        //    {
        //        _coworkerRepo = new CoworkerRepo(_npgsqlConnection);
        //    }

        //    return _coworkerRepo;
        //}

        //public static ProjectRepo GetProjectRepo()
        //{
        //    if (_projectRepo == null)
        //    {
        //        _projectRepo = new ProjectRepo(_npgsqlConnection);
        //    }

        //    return _projectRepo;
        //}

        //public static TaskRepo GetTaskRepo()
        //{
        //    if (_taskRepo == null)
        //    {
        //        _taskRepo = new TaskRepo(_npgsqlConnection);
        //    }

        //    return _taskRepo;
        //}

    }
}
