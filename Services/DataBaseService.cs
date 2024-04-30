using Dapper;
using LineBot_Sample.Models;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace LineBot_Sample.Services
{
    public interface IDataBaseService
    {
        List<SettingDic> GetSetting();
    }
    public class DataBaseService : IDataBaseService
    {
        public static SqlConnection _db;
        public DataBaseService(IOptions<ConnStr> ConnStr) {
            _db = new SqlConnection(ConnStr.Value.DB);
        }

        public List<SettingDic> GetSetting()
        {
            string sqlCmd = "SELECT * FROM [LineBotSetting]";

            return _db.Query<SettingDic>(sqlCmd).ToList();
        }
    }
}
