using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerFolderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var emailConfig = hostContext.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
                    services.AddSingleton(emailConfig);
                    services.AddSingleton<IEmailSender, EmailSender>();
                    services.AddHostedService<Worker>();
                });
    }
}
