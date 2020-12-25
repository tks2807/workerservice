using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerFolderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _sender;
        private FileSystemWatcher watcher;
        private readonly string path = @"/Users/imac/Desktop/folder";  

        public Worker(ILogger<Worker> logger, IEmailSender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Changed += OnChanged;
            return base.StartAsync(cancellationToken);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("New file created at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "jewandfriends@gmail.com" }, "SE1902 subject", "Create File on Worker Service", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File deleted at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "jewandfriends@gmail.com" }, "SE1902 subject", "Delete File on Worker Service", null);
            _sender.SendEmail(message);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _logger.LogInformation("File renamed at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "jewandfriends@gmail.com" }, "SE1902 subject", $"Rename File on Worker Service, old path: {e.OldFullPath}", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File changed at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "jewandfriends@gmail.com" }, "SE1902 subject", $"Change File on Worker Service, changed: {e.ChangeType}", e.FullPath);
            _sender.SendEmail(message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                watcher.EnableRaisingEvents = true;
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
