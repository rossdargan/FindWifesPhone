using System.Threading.Tasks;

namespace FindWifesPhone.Library
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class FindPhoneService : IFindPhoneService, IDisposable
    {
        private readonly string _clientId;
        private const string ClientBuildNumber = "15G78";
        private const string ICloudSetupUrl = "https://setup.icloud.com/setup/ws/1/login";
        private const string ICloudPlaySoundUrl = "/fmipservice/client/web/playSound";
        private const string ICloudInitClientUrl = "/fmipservice/client/web/initClient";
        
        private readonly WebClient _webClient;
        public FindPhoneService(string clientId)
        {
            _clientId = clientId;

            _webClient = new WebClient();
            _webClient.Headers.Add("Origin", "https://www.icloud.com");
            _webClient.Headers.Add("Content-Type", "text/plain");
        }

        public async Task<bool> FindPhone(string username, string password, string deviceName)
        {
            string urlParams = $"?clientBuildNumber={ClientBuildNumber}&clientId={_clientId}";
      
          
            var loginData = await Login(username, password, urlParams);
                        
            dynamic loginDataResult = JObject.Parse(loginData);
            string iCloudDeviceUrl = loginDataResult.webservices.findme.url;
            string dsid = loginDataResult.dsInfo.dsid;

            urlParams += $"&dsid={dsid}";            

            string clientContext = "\"clientContext\":{\"appName\":\"iCloud Find (Web)\",\"appVersion\":\"2.0\","
                                   + "\"timezone\":\"US/Pacific\",\"inactiveTime\":2255,\"apiVersion\":\"3.0\",\"fmly\":true}";

            var jsonString = await _webClient.PostDataToWebsiteAsync(iCloudDeviceUrl + ICloudInitClientUrl + urlParams, "{" + clientContext + "}");

            if (jsonString.StartsWith("{\"statusCode\":\"200\""))
            {
                dynamic response = JObject.Parse(jsonString);

                JObject serverContext = response.serverContext;
                string jsonServerContext = $"\"serverContext\":{serverContext.ToString(Formatting.None)}";
                var devices = response.content;                 
                foreach (var device in devices)
                {
                    if (device.name == deviceName)
                    {
                        string id = device.id;
                        string json = "{\"device\":\"" + id + "\", \"subject\":\"Find My iPhone Alert\", " + clientContext + ", " + jsonServerContext + "}";
                        await _webClient.PostDataToWebsiteAsync(
                            iCloudDeviceUrl + ICloudPlaySoundUrl + urlParams, json);

                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<string> Login(string username, string password,string urlParams)
        {
            string iCloudLoginUrl = $"{ICloudSetupUrl}{urlParams}";


            dynamic loginRequest= new ExpandoObject();
            loginRequest.apple_id = username;
            loginRequest.password = password;
            loginRequest.extended_login = false;

            string loginPostData = JsonConvert.SerializeObject(loginRequest);

            var loginData = await _webClient.PostDataToWebsiteAsync(iCloudLoginUrl, loginPostData);

            if (_webClient.ResponseHeaders.AllKeys.Any(k => k == "Set-Cookie"))
            {
                _webClient.Headers.Add("Cookie", _webClient.ResponseHeaders["Set-Cookie"]);
            }
            else
            {
                throw new System.Security.SecurityException("Invalid username / password");
            }


            return loginData;
        }

        public void Dispose()
        {

            this._webClient?.Dispose();
        }
    }
}
