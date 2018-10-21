using System;
using System.Net;
using System.ServiceProcess;
using JetBlack.MessageBus.FeedBus.Distributor.Configuration;
using log4net;
using Microsoft.Extensions.Configuration;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.Devlopment.config")]

namespace JetBlack.MessageBus.FeedBus.Distributor
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            var server = CreateServer(args);

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

            server.Dispose();
        }

        static Server CreateServer(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            var programArgs = ProgramArgs.Parse(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();
            var config = configuration.Get<DistributorConfig>();

            var endPoint = new IPEndPoint(config.Address, config.Port);

            var server = new Server(endPoint);
            server.Start(config.HeartbeaInterval);

            return server;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal($"Unhandled error received - IsTerminating={args.IsTerminating}", args.ExceptionObject as Exception);
        }
    }
}
