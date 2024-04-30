namespace LineBot_Sample.Models
{
    public class SettingModel
    {
        public string PixivCookie { get; set; } = string.Empty;

        public string ImgurID { get; set; } = string.Empty;
    }

    public class SettingDic
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
