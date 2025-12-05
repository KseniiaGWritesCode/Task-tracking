using Newtonsoft.Json;

namespace TaskTracking
{
    public class AppConfig
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _database;
        private readonly string _username;
        private readonly string _password;

        public AppConfig(string host, int port, string database, string username, string password)
        {
            _database = database;
            _host = host;
            _port = port;
            _username = username;
            _password = password;
        }

        public AppConfig(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("AppConfig file not found, prepare this file before stations this app!");
            }

            var json = File.ReadAllText(fileName);

            var appConfig = JsonConvert.DeserializeObject<AppConfigDTO>(json);

            _database = appConfig.Database;
            _host = appConfig.Host;
            _port = appConfig.Port;
            _username = appConfig.Username;
            _password = appConfig.Password;
        }

        public string GetConnectionString()
        {
            return $"Host={_host};Port={_port};Database={_database};Username={_username};Password={_password};Include Error Detail=true;";
        }
    }
}
