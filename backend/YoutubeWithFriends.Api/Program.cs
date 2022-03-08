using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace YoutubeWithFriends.Api {
    public class Program {
        internal const string USER_SESSION_ID_COOKIE_NAME = "userSessionId";
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());
    }
}