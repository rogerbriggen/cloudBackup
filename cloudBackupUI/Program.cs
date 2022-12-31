// Roger Briggen licenses this file to you under the MIT license.
//

using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CloudBackupUI;

class Program
{

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        System.Threading.Thread.CurrentThread.Name = "MainThread";

        //Setup log with DI
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();
        var assembly = typeof(Program).Assembly;
        logger?.LogInformation("Application with version " + assembly.GetName().Version + " started...");

        //setup UI
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());

        //Finish log
        logger?.LogInformation("Application closing...");
        //Use for Microsoft logger.... for serilog, we do it with AppDomain.CurrentDomain.ProcessExit
        //serviceProvider.Dispose();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        //we will configure logging here
        //Microsoft logger
        /*
        services.AddLogging(configure => configure.AddConsole(consoleLoggerOptions => consoleLoggerOptions.TimestampFormat = "[dd.MM.yyyy HH:mm:ss.fff]")).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug).AddTransient<Program>();
        services.AddLogging(configure => configure.AddDebug()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug).AddTransient<Program>();
        */

        //Serilog logger from code
        /*
        string outputTemplate = "[{Timestamp:dd.MM.yyyy HH:mm:ss.ffff} {Level:u3}] {Message:lj} {Properties}[{ThreadId} {ThreadName}]{NewLine}{Exception}";
        services.AddSerilogServices(new LoggerConfiguration()
                                            .Enrich.WithThreadId()
                                            .Enrich.WithThreadName()
                                            .MinimumLevel.Debug()
                                            .WriteTo.Console(outputTemplate: outputTemplate, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                                            .WriteTo.Async(w => w.File("MyLog.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate))
                                            .WriteTo.Debug(outputTemplate: outputTemplate));

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true)).AddTransient<Program>();
        */


        //Serilog logger from file
        var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();
        //Get application stuff
        //var RemoteIncrediBuildSec = configuration.GetSection("RemoteIncrediBuild");
        /*
        if (RemoteIncrediBuildSec != null)
        {
            bool? bIsServer = RemoteIncrediBuildSec.GetValue<bool?>("IsServer");
            if (bIsServer == true)
            {
                ConfigFileAppSettings.Action = AppSettings.EAction.Server;
            }
            else if (bIsServer == false)
            {
                ConfigFileAppSettings.Action = AppSettings.EAction.Client;
            }

        }
        */


        services.AddSerilogServices(new LoggerConfiguration()
                                            .ReadFrom.Configuration(configuration));

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true)).AddTransient<Program>();
        Log.Information("**** Opening log... ****");
    }

}



public static class Extensions
{
    public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
    {
        Log.Logger = configuration.CreateLogger();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => { Log.Information("**** Closing log... ****"); Log.CloseAndFlush(); };
        return services.AddSingleton(Log.Logger);
    }
}

