using System;
using System.Net;
using System.ServiceProcess;
using log4net;
using Microsoft.Extensions.Configuration;
using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.FeedBus.Distributor.Configuration;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

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

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var distributorSection = configuration.GetSection("distributor");
            var distributorConfig = distributorSection.Get<DistributorConfig>();

            var endPoint = new IPEndPoint(distributorConfig.Address.AsIPAddress(), distributorConfig.Port);

            var server = new Server(endPoint);
            server.Start(distributorConfig.HeartbeatInterval);

            return server;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal($"Unhandled error received - IsTerminating={args.IsTerminating}", args.ExceptionObject as Exception);
        }
    }
}
