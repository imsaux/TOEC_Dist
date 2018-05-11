using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TOEC_Dist
{
    public class Config
    {
        public static string GetAppConfig(string strKey)
        {
            ExeConfigurationFileMap map_Base = new ExeConfigurationFileMap();
            map_Base.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"Config\Base.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map_Base, ConfigurationUserLevel.None);
            return config.AppSettings.Settings[strKey].Value;
            //foreach (string key in ConfigurationManager.AppSettings)
            //{
            //    if (key == strKey)
            //    {
            //        return ConfigurationManager.AppSettings[strKey];
            //    }
            //}

        }
        public static bool SetAppConfig(string key, string value)
        {
            try
            {
                ExeConfigurationFileMap map_Base = new ExeConfigurationFileMap();
                map_Base.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"Config\Base.config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map_Base, ConfigurationUserLevel.None);
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                //Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                //AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings");
                //appSetting.Settings[key].Value = value;
                //config.Save();//保存web.config  
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}