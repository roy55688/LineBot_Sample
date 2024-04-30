# LineBot_Sample
Built with the Line.Messaging.NETCore package, the LineBot can parse links containing images from Pixiv and Twitter and convert them into Bubble Messages.

# LineBot使用方法
1. 需先至 https://developers.line.biz/zh-hant/ 建立 MessageAPI。
1. 將申請到的 AccessToken 與 ChannelSecret 放入程式使用。(本範例是放入環境變數使用)
1. 本地端測試可使用 ngrok 等程式將開發環境的連線對外以設定Webhook URL。
1. 因 LineBot 的 Webhook URL 固定，因此本範例使用 minimal API 來管理路由，並將流程控制層移置 Application。
1. 本範例使用的套件為 https://www.nuget.org/packages/Line.Messaging.NETCore/


## TwitterService
1. 此範例調用第三方 API 來解析 tweet 的圖片僅為測試使用，請勿直接使用在生產環境，或取得 API 提供者的授權。
1. TwitterService 無其他額外參數設定，可直接進行測試。
1. 使用範例如下 :
![image](https://github.com/roy55688/LineBot_Sample/blob/main/LineBot_Sample_twitter.jpg?raw=true)

## PixivService
1. 此範例使用 cookie 來模擬登入者身分，並將圖片上傳至 Imgur 後以取得圖片 url 來製作 Line 回復訊息，因此需要設定以下參數。
1. 登入 cookie 可以於登入 pixiv 網站時從瀏覽器上取得。
1. Imgur 需註冊並申請 Client ID 使用。
1. 使用範例如下 :
![image](https://github.com/roy55688/LineBot_Sample/blob/main/LineBot_Sample_pixiv.jpg?raw=true)

### 本範例純粹以學術交流與個人實作練習為主，若有任何不妥請聯繫本人進行移除。