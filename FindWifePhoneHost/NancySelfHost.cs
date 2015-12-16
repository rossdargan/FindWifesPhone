namespace FindWifePhoneHost
{
    using System;

    using Nancy.Hosting.Self;

    /// <summary>
    /// Responsible for starting and stopping Nancy.
    /// </summary>
    public class NancySelfHost
    {
        /// <summary>
        /// The base url for the service.
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// The nancy host.
        /// </summary>
        private NancyHost _nancyHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancySelfHost"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        public NancySelfHost(string baseUrl)
        {
            this._baseUrl = baseUrl;
        }

        /// <summary>
        /// Starts Nancy listening for requests
        /// </summary>
        public void Start()
        {
            this._nancyHost = new NancyHost(new Uri(this._baseUrl));
            this._nancyHost.Start();
            Console.WriteLine($"Find Wife Phone service host is now listening - {this._baseUrl}. Press ctrl-c to stop");
        }

        /// <summary>
        /// Stops Nancy from listening for requests.
        /// </summary>
        public void Stop()
        {
            this._nancyHost.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }
}