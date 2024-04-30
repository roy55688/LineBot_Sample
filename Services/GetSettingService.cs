using LineBot_Sample.Models;

namespace LineBot_Sample.Services
{
    public interface IGetSettingService
    {
        SettingModel Setting { get; }
        void GetSetting();
    }
    public class GetSettingService : IGetSettingService
    {
        private static IDataBaseService _db;

        private SettingModel _settingModel;
        public SettingModel Setting => _settingModel;

        public GetSettingService(IDataBaseService dataBaseService)
        {
            _db = dataBaseService;
            GetSetting();
        }

        public void GetSetting()
        {
            SettingModel settingModel = new SettingModel();

            List<SettingDic> settingDics = _db.GetSetting();

            foreach (SettingDic settingDic in settingDics)
                settingModel.GetType().GetProperty(settingDic.Key)?.SetValue(settingModel, settingDic.Value);

            _settingModel = settingModel;
        }
    }
}
