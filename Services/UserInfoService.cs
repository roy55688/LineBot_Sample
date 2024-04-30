using System.Text.Json;
using static LineBot_Sample.Services.UserInfoService;

namespace LineBot_Sample.Services
{
    public interface IUserInfoService
    {
        public UserModel GetUserInfo(string userId);
        public string GetUserName(string userId);
    }
    public class UserInfoService : IUserInfoService
    {
        private readonly string _accessToken;
        public UserInfoService(IConfiguration config)
        {
            _accessToken = config["LineBot_AccessToken"];
        }
        public class UserModel
        {
            public string userId { get; set; }
            public string displayName { get; set; }
            public string pictureUrl { get; set; }
        }
        public UserModel GetUserInfo(string userId)
        {
            string url = $"https://api.line.me/v2/bot/profile/{userId}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = client.GetAsync(url).Result;
            string result = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<UserModel>(result);
        }
        public string GetUserName(string userId)
        {
            string url = $"https://api.line.me/v2/bot/profile/{userId}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage = client.GetAsync(url).Result;
            string result = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<UserModel>(result).displayName;
        }
    }
}
