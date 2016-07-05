using System.Threading.Tasks;

namespace FindWifesPhoneDNX.Library
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class FindPhoneService : IFindPhoneService, IDisposable
    {
        private readonly string _clientId;

        private readonly string username;

        private readonly string password;

        private const string ClientBuildNumber = "15G78";
        private const string ICloudSetupUrl = "https://setup.icloud.com/setup/ws/1/login";
        private const string ICloudPlaySoundUrl = "/fmipservice/client/web/playSound";
        private const string ICloudInitClientUrl = "/fmipservice/client/web/initClient";
        
        private readonly HttpClient _webClient;

        private string _authCookie;

        public FindPhoneService(string clientId, string username, string password)
        {
            _clientId = clientId;
            this.username = username;
            this.password = password;

            _webClient = new HttpClient();               
            _webClient.DefaultRequestHeaders.Add("Origin", "https://www.icloud.com");            
        }

        public async Task<bool> FindPhone(string deviceName)
        {
            string urlParams = $"?clientBuildNumber={ClientBuildNumber}&clientId={_clientId}";
      
          
            var loginData = await Login(username, password, urlParams);
                        
            dynamic loginDataResult = JObject.Parse(loginData);
            string iCloudDeviceUrl = loginDataResult.webservices.findme.url;
            string dsid = loginDataResult.dsInfo.dsid;

            urlParams += $"&dsid={dsid}";            

            string clientContext = "\"clientContext\":{\"appName\":\"iCloud Find (Web)\",\"appVersion\":\"2.0\","
                                   + "\"timezone\":\"US/Pacific\",\"inactiveTime\":2255,\"apiVersion\":\"3.0\",\"fmly\":true}";

            var jsonString = await PostDataToWebsiteAsync(_webClient, iCloudDeviceUrl + ICloudInitClientUrl + urlParams, "{" + clientContext + "}");

            if (jsonString.EndsWith("\"statusCode\":\"200\"}"))
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
                        await PostDataToWebsiteAsync(_webClient,
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

            var loginData = await PostDataToWebsiteWithResponseAsync(_webClient, iCloudLoginUrl, loginPostData);

            if (loginData.IsSuccessStatusCode)
            {
                _authCookie = loginData.Headers.FirstOrDefault(p => p.Key == "Set-Cookie").Value.First();
            }
            else
            {
                throw new System.Security.SecurityException("Invalid username / password");
            }


            return await loginData.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {

            this._webClient?.Dispose();
        }

        private async Task<HttpResponseMessage> PostDataToWebsiteWithResponseAsync(HttpClient wc, string url, string postData)
        {


            var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
    
            if (!string.IsNullOrWhiteSpace(_authCookie))
            {
                requestMessage.Headers.Add("Cookie", _authCookie);
                //_webClient.Headers.Add("Cookie", _webClient.ResponseHeaders["Set-Cookie"]);               
            }

            var result = await wc.PostAsync(url, content);

            string retVal = await result.Content.ReadAsStringAsync();

            return result;
        }
        private async Task<string> PostDataToWebsiteAsync(HttpClient wc, string url, string postData)
        {

            string retVal = await PostDataToWebsiteWithResponseAsync(wc, url, postData).Result.Content.ReadAsStringAsync();

            return retVal;
        }
    }
}
