using AngleSharp.Html.Parser;
using Line.Messaging;
using Line.Messaging.Messages;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.DevTools.V118.Debugger;
using System.IO;
using System.Net.Http;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace LineBot_Sample.Services
{
    public interface ITwitterService
    {
        Task<List<TextMessage>> SaveTwitterImage(string path, string sourceurl);

        Task<FlexMessage> CreateFlexMessage(string url);
    }
    public class TwitterService : ITwitterService
    {
        public TwitterService()
        {}

        public async Task<List<TextMessage>> SaveTwitterImage(string path, string sourceurl)
        {
            List<TextMessage> result = new List<TextMessage>();

            List<string> urls = await GetPicutreurls(sourceurl);

            HttpClient httpClient = new HttpClient();

            foreach (string url in urls)
            {
                var responseMessage = await httpClient.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var stream = responseMessage.Content.ReadAsStream();
                    string fileName = Path.GetFileName(url);

                    var ar = fileName.Split('?');

                    var arr = ar[1].Split('=');

                    string realFileName = ar[0] + "." + arr[1];

                    string filePath = Path.Combine(path, $"{realFileName}");
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        stream.CopyTo(fs);
                    }
                    result.Add(new TextMessage($"圖片已存入{filePath}"));
                }
            }
            return result;
        }

        public async Task<FlexMessage> CreateFlexMessage(string url)
        {
            TweetModel tweet = await GetTweetsInfo(url);

            string pictureurl = tweet.pictureUrl;
            string sourceurl = url;
            string poster = tweet.poster;

            #region 取出內文

            List<string> innerList = new List<string>();

            var parser = new HtmlParser();

            var doc = parser.ParseDocument(tweet.textHtml ?? "");

            var selector = doc.QuerySelectorAll("span");

            for (int i = 0; i < selector.Count(); i++)
            {
                var item = selector[i];
                if (item.ChildElementCount == 0) //純文字
                {
                    string inner = item.InnerHtml;
                    if (!string.IsNullOrEmpty(inner))
                    {
                        innerList.Add(inner);
                        //AddToContainerBody(ref container, inner);
                    }
                }
                else
                {
                    foreach (var child in item.Children)
                    {
                        var a = child.InnerHtml;
                        if (!string.IsNullOrEmpty(a))
                        {
                            innerList.Add(a);
                            //AddToContainerBody(ref container, inner);
                        }
                    }
                }
            }
            #endregion

            var container = BubbleFactory.CreateImageBubble(AspectRatio._3_4, pictureurl, poster, "Twitter", url, innerList.ToArray());

            FlexMessage flexMessage = new FlexMessage(poster) { Contents = container };

            return flexMessage;
        }

        private async Task<List<string>> GetPicutreurls(string url)
        {
            List<string> result = new List<string>();

            string baseurl = "https://tweethunter.io/api/v2/tweets?url=";

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Referer", "https://tweethunter.io/tweetpik/twitter-downloader");

            HttpResponseMessage response = await client.GetAsync(baseurl + url);

            string content = await response.Content.ReadAsStringAsync();

            content = content.TrimStart('[');
            content = content.TrimEnd(']');

            JObject jsonObject = JObject.Parse(content);

            JArray array = (JArray)jsonObject["photos"];

            foreach (var item in array)
            {
                string puctureurl = item.ToString();
                var splitUrl = puctureurl.Split('?');
                var splitQuery = splitUrl[1].Split('&');
                string realurl = splitUrl[0] + "?" + splitQuery[0];
                result.Add(realurl);
            }
            return result;
        }

        private async Task<TweetModel> GetTweetsInfo(string url)
        {
            TweetModel result = new TweetModel();
            string baseurl = "https://tweethunter.io/api/v2/tweets?url=";

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Referer", "https://tweethunter.io/tweetpik/twitter-downloader");

            HttpResponseMessage response = await client.GetAsync(baseurl + url);

            string content = await response.Content.ReadAsStringAsync();

            content = content.TrimStart('[');
            content = content.TrimEnd(']');

            JObject jsonObject = JObject.Parse(content);

            JArray photoArray = (JArray)jsonObject["photos"];

            if (photoArray.Count > 0)
            {
                string puctureurl = photoArray[0].ToString();

                result.pictureUrl = puctureurl;

            }

            result.poster = jsonObject["handler"].ToString();

            result.textHtml = jsonObject["textHtml"]?.ToString();


            return result;
        }

        public class TweetModel
        {
            public string? textHtml { get; set; }
            public string pictureUrl { get; set; } = string.Empty;
            public string poster { get; set; }
        }
    }
}
