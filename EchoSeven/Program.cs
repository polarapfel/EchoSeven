using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EchoSevenUtility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mono.Unix;
using Mono.Unix.Native;
using static Mono.Unix.UnixFileInfo;

namespace EchoSeven
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(OSPlatform.Linux))
            {
                // Todo: Make dependencies to Mono.Unix optional and runtime OS dependent and support similar APIs on Windows and Mac OS X
                Console.Error.WriteLine("Unsupported platform. " +
                                        "EchoSeven has Linux specific dependencies. Sorry!");
                Environment.Exit(1);
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // see chmod(2), https://linux.die.net/man/2/chmod for reference
                    FilePermissions root600 =
                        FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IFREG;
                    FilePermissions root640 =
                        FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IRGRP | FilePermissions.S_IFREG;
                    
                    if (File.Exists("/etc/EchoSeven/appsettings.json") 
                        && UnixFileSystemInfo.GetFileSystemEntry("/etc/EchoSeven/appsettings.json").OwnerUser.UserName.Equals("root")
                        && UnixFileSystemInfo.GetFileSystemEntry("/etc/EchoSeven/appsettings.json").OwnerGroup.GroupName.Equals("root")
                        && (UnixFileSystemInfo.GetFileSystemEntry("/etc/EchoSeven/appsettings.json").Protection.Equals(root600) 
                        || UnixFileSystemInfo.GetFileSystemEntry("/etc/EchoSeven/appsettings.json").Protection.Equals(root640))
                        && UnixEnvironment.EffectiveUser.UserName.Equals("root"))
                    {
                        var settings = config.Build();
                        config.AddJsonFile("/etc/EchoSeven/appsettings.json", optional: true, reloadOnChange: false);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    EchoServerOptions options = configuration.GetSection("EchoService").Get<EchoServerOptions>();
                    services.AddSingleton(options);
                    services.AddHostedService<EchoSevenService>();
                });
    }
}