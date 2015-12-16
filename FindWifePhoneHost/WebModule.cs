namespace SensorHost
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using FindWifesPhone.Library;

    using Nancy;
    

    public class WebModule : NancyModule
    {
        private readonly IFindPhoneService _findPhoneService;

        /// <summary>
        /// Creates a webmodule listening to the get requests for all the sensors.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="sensorFactory"></param>
        public WebModule()
        {
            _findPhoneService = new FindPhoneService("213-123-123");
            Username = ConfigurationManager.AppSettings["UserName"];
            Password = ConfigurationManager.AppSettings["Password"];
            DefaultDevice = ConfigurationManager.AppSettings["DefaultDevice"];

            string postPath = "/findphone/{device}";
            Console.WriteLine($"Listening for post on {postPath}");
            Post[postPath] = paramaters => FindPhone(paramaters.device);
            string postPathDefault = "/findphone";

            Post[postPathDefault] = paramaters => FindPhone(DefaultDevice);

        }

        private async Task<int> FindPhone(string device)
        {
            try
            {
                bool result = await _findPhoneService.FindPhone(Username, Password, device);
                return result ? 200 : 500;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                return 502;
            }

        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DefaultDevice { get; set; }

     
    }
}