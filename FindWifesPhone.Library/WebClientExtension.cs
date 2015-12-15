namespace FindWifesPhone.Library
{
    using System.Net;
    using System.Threading.Tasks;

    public static class WebClientExtension
    {
        public static Task<string> PostDataToWebsiteAsync(this WebClient wc, string url, string postData)
        {
            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            var result = wc.UploadStringTaskAsync(url, "POST", postData);

            return result;
        }
    }
}

