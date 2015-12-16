using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindWifePhoneHost
{
    using System.Configuration;

    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {

            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
          

            HostFactory.Run(x =>
            {
                x.Service<NancySelfHost>(s =>
                {
                    s.ConstructUsing(name => new NancySelfHost(baseUrl));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("A microservice to provide PRTG with information from the various sensors in the sensors folder");
                x.SetDisplayName("PRTG Sensor Host");
                x.SetServiceName("SensorHost");
            });
        }
    }
}
