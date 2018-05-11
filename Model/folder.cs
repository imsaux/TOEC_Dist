using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heng;
using System.Security.AccessControl;

namespace TOEC_Dist.Model
{
    public class folder : software
    {
        public folder()
        {
            subfolder = new List<string>();
        }

        public string path { get; set; }
        public List<string> subfolder { get; set; }

        /// <summary>
        /// 部署
        /// </summary>
        /// <returns></returns>
        public override void Deploy(string stnm = "", string tcode = "", string ip = "")
        {
            CreatFolder();
            ShareFolder();
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <returns></returns>
        public bool CreatFolder()
        {
            try
            {
                //创建根目录
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //创建子目录
                foreach (string sub in subfolder)
                {
                    if (!Directory.Exists(path + "\\" + sub))
                    {
                        Directory.CreateDirectory(path + "\\" + sub);
                    }
                }
                report.Add("创建" + name, "成功"); //创建
                return true;
            }
            catch (Exception e)
            {
                report.Error("创建文件夹异常", e);
                return false;
            }

        }
        /// <summary>
        /// 共享文件夹
        /// </summary>
        /// <returns></returns>
        public bool ShareFolder()
        {
            try
            {
                Helper_FileDir.AddSecurityControll2Folder(path);
                Helper_FileDir.ShareFolder(path.TrimEnd('\\'), name);
                report.Add("共享" + name, "成功");
                return true;
            }
            catch (Exception e)
            {
                report.Error("共享文件夹异常", e);
                return false;
            }

        }
    }
}
