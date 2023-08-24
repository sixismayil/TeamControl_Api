using Microsoft.Extensions.Configuration;
using System.IO;


namespace TeamControlV2
{
    public class AppConfiguration
    {
        private readonly string _jwtSecret = string.Empty;
        private readonly string _conString = string.Empty;
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _jwtSecret = root.GetSection("ApplicationSettings").GetSection("jwt_secret").Value;

            //ConnectionString
            _conString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }
      
        public string ConnectionString
        {
            get => _conString;
        }
        public string JWTSecret
        {
            get => _jwtSecret;
        }
    }
}
