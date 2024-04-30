using Line.Messaging;
using Line.Messaging.Messages;
using Line.Messaging.Webhooks;
using LineBot_Sample.Models;
using LineBot_Sample.Services;
using OpenQA.Selenium.DevTools.V118.ServiceWorker;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LineBot_Sample.Applications
{
    public interface ILineBotApplication
    {
        public Task RunAsync(IEnumerable<WebhookEvent> webhookEvents);
    }
    public class LineBotApplication : WebhookApplication, ILineBotApplication
    {
        private LineMessagingClient _messagingClient;
        private IPixivService _pixivService;
        private ITwitterService _twitterService;
        private IImgurService _imgurService;
        private IGetSettingService _getSettingService;

        public LineBotApplication(LineMessagingClient lineMessagingClient, IPixivService pixivService, ITwitterService twitterService, IImgurService imgurService, IGetSettingService getSettingService)
        {
            _messagingClient = lineMessagingClient;
            _pixivService = pixivService;
            _twitterService = twitterService;
            _imgurService = imgurService;
            _getSettingService = getSettingService;
        }

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            List<ISendMessage> result = new List<ISendMessage>();

            string userId = ev.Source.UserId;
            string channelId = ev.Source.Id;

            switch (ev.Message)
            {
                case TextEventMessage textMessage:
                    string t = textMessage.Text;
                    //P網連結
                    if (t.Contains("https://www.pixiv"))
                    {
                        string[] ts = t.Split(' ');

                        string url = ts.Where(x => x.Contains("https://www.pixiv")).First().Trim();

                        (string,Stream) pictureAuthorAndStream = await _pixivService.GetPixivImageAuthorAndStream(url);

                        if (pictureAuthorAndStream.Item2 is not null)
                        {
                            string pictureUrl = await _imgurService.UploadToImgurAsync(pictureAuthorAndStream.Item2);

                            var bubble = BubbleFactory.CreateImageBubble(AspectRatio._3_4, pictureUrl, pictureAuthorAndStream.Item1, "Pixiv", url);

                            result.Add(new FlexMessage(pictureAuthorAndStream.Item1) { Contents = bubble });
                        }
                        else
                        {
                            _getSettingService.GetSetting();
                        }


                    }
                    //twitter連結
                    if (t.Contains("https://x.com") || t.Contains("https://twitter.com/"))
                    {
                        var twitterReturn = await _twitterService.CreateFlexMessage(t);

                        if (twitterReturn is not null) result.Add(twitterReturn);
                    }

                    break;
                case ImageEventMessage imageMessage:
                    //if (imageMessage.ContentProvider.Type != ContentProviderType.Line) break;
                    
                    break;
            }

            if (0 < result.Count && result.Count <= 5)
                await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
            else if (result.Count > 5)
                await _messagingClient.ReplyMessageAsync(ev.ReplyToken, $"機器人回應大於單次上限，本次共{result.Count}筆回應。");

        }
    }
}
