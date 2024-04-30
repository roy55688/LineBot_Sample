using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.Extensions.Options;
using Line.Messaging;
using System.IO;
using System.Net.Http;
using LineBot_Sample.Models;

namespace LineBot_Sample.Services
{
    public interface IPixivService
    {
        Task<List<TextMessage>> SavePixivImage(string path, string url);

        Task<(string, Stream)> GetPixivImageAuthorAndStream(string url);
    }
    public class PixivService : IPixivService
    {
        readonly string _cookie;
        public PixivService(IGetSettingService settingService) 
        {
            _cookie = settingService.Setting.PixivCookie;
        }

        public async Task<(string,Stream)> GetPixivImageAuthorAndStream(string url)
        {
            string id = Path.GetFileName(url);

            string ajaxurl = $"https://www.pixiv.net/ajax/illust/{id}";
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Cookie", _cookie);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0");

            HttpResponseMessage ajaxResponse = await client.GetAsync(ajaxurl);

            string ajaxContent = await ajaxResponse.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(ajaxContent);

            var body = jsonObject["body"];

            var authorId = body["tags"]["authorId"].ToString();

            var pictureurl = body["urls"]["original"].ToString();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Referer", url);

            var responseMessage = await httpClient.GetAsync(pictureurl);

            if (responseMessage.IsSuccessStatusCode)
            {
                var stream = responseMessage.Content.ReadAsStream();
                return (authorId,stream);
            }
            else
            {
                return (authorId, null);
            }

        }

        public async Task<List<TextMessage>> SavePixivImage(string path, string url)
        {
            List<TextMessage> result = new List<TextMessage>();
            
            string id = Path.GetFileName(url);

            string ajaxurl = $"https://www.pixiv.net/ajax/illust/{id}";
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Cookie", _cookie);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0");

            HttpResponseMessage ajaxResponse = await client.GetAsync(ajaxurl);

            string ajaxContent = await ajaxResponse.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(ajaxContent);

            var body = jsonObject["body"];

            var authorId = body["tags"]["authorId"].ToString();

            var pictureurl = body["urls"]["original"].ToString();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Referer", url);

            //"/images"

            int pictureCount = 0;
            while (true)
            {
                string pictureURL = pictureurl.Replace("_p0", $"_p{pictureCount}");
                var responseMessage = await httpClient.GetAsync(pictureURL);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var stream = responseMessage.Content.ReadAsStream();
                    string fileName = Path.GetFileName(pictureURL);

                    string filePath = Path.Combine(path, $"{authorId}_{fileName}");
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        stream.CopyTo(fs);
                    }
                    result.Add(new TextMessage($"圖片已存入{filePath}"));
                }
                else
                    break;
                pictureCount++;
            }

            return result;
        }
    }
}
