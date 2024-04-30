using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using LineBot_Sample.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace LineBot_Sample.Services
{
    public interface IImgurService
    {
        Task<string> UploadToImgurAsync(Stream stream);
    }
    public class ImgurService : IImgurService
    {
        readonly string clientID;
        public ImgurService(IGetSettingService settingService) {
            clientID = settingService.Setting.ImgurID;
        }
        public async Task<string> UploadToImgurAsync(Stream stream)
        {
            string result = string.Empty;
            string apiUrl = "https://api.imgur.com/3/image";
            // 使用HttpClient進行POST請求
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientID);

                content.Add(new StreamContent(stream), "image");

                // 發送POST請求
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 檢查是否請求成功
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(responseContent);

                    result = jsonObject["data"]["link"].ToString();
                }
                else
                {
                    Console.WriteLine($"失敗：StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");
                    // 輸出詳細錯誤訊息
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"詳細錯誤訊息：{errorContent}");
                }
            }

            return result;
        }
    }
}
