using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Topshelf;

namespace EventStoreService
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = (EventStoreServiceConfiguration)ConfigurationManager.GetSection("eventStore");
            var address = GetIPAddress();

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.EnableShutdown();
                x.EnableServiceRecovery(c => c.RestartService(1));

                x.Service<EventStoreProcessWrapper>(s =>
                {
                    s.ConstructUsing(name => new EventStoreProcessWrapper(address, configuration.Instances));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetDescription("EventStore Wrapping Service");
                x.SetDisplayName("EventStore");
                x.SetServiceName("EventStore");
            });

            Console.ReadLine();
        }

        private static IPAddress GetIPAddress()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).First(a => a.AddressFamily == AddressFamily.InterNetwork && !a.Equals(IPAddress.Loopback));
        }
    }
}
