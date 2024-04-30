using Line.Messaging;
using Line.Messaging.Webhooks;
using LineBot_Sample.Applications;
using LineBot_Sample.Models;
using LineBot_Sample.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    
    builder.Services.Configure<ConnStr>(builder.Configuration.GetSection("ConnectionStrings"));

    builder.Services.AddSingleton<IDataBaseService,DataBaseService>();

    builder.Services.AddSingleton<IGetSettingService, GetSettingService>();

    //取用環境變數
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSingleton<LineMessagingClient>(provider =>
    {
        var environment = Environment.GetEnvironmentVariable("LineBot_AccessToken");
        return new LineMessagingClient(environment);
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ILineBotApplication, LineBotApplication>();

    builder.Services.AddScoped<IUserInfoService, UserInfoService>();
    builder.Services.AddScoped<IImgurService, ImgurService>();
    builder.Services.AddScoped <IPixivService,PixivService>();
    builder.Services.AddScoped<ITwitterService, TwitterService>();

    var app = builder.Build();

    app.MapPost("/linebot", async (IServiceProvider serviceProvider, ILineBotApplication lineBotApp) =>
    {
        HttpContext _httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        var channelSecret = Environment.GetEnvironmentVariable("LineBot_ChannelSecret");
        var events = await _httpContext.Request.GetWebhookEventsAsync(channelSecret);
        await lineBotApp.RunAsync(events);
        return Results.NoContent();
    });

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

