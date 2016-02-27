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
                x.SetDescription("A microservice used to log in to apples iCloud and locate a device");
                x.SetDisplayName("Find an iPhone");
                x.SetServiceName("FindWifesPhone");
            });
        }
    }
}
