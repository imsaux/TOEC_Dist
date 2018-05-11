using Heng;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TOEC_Dist.Model
{
    /// <summary>
    /// 服务
    /// </summary>
    public class server : software
    {
        /// <summary>
        /// 部署
        /// </summary>
        /// <param name="stnm"></param>
        /// <param name="tcode"></param>
        /// <param name="ip"></param>
        public override void Deploy(string stnm, string tcode, string ip)
        {
            bool flag = false;
            try
            {
                //特殊处理：清理老版不规范的服务
                Delete_Old_Software();

                //判断服务是否存在
                if (Helper_Service.Service_IsExisted(name))
                {
                    #region 升级
                    report.Add(comment + "升级...");
                    if (!Helper_Service.Server_Close(name)) //先关闭服务
                    {
                        report.Error(name + "服务关闭失败");
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        report.Add(name + "服务已关闭");

                        report.Add("检查残留进程...");
                        if (Helper_Process.CheckProcessExists(name))
                        {
                            if (Helper_Process.KillProcess(name))
                            {
                                report.Add("进程已经被强制结束");
                            }
                            else
                            {
                                report.Error("进程强制结束失败");
                            }
                        }
                        flag = Update(stnm, tcode, ip);
                    }
                    #endregion
                }
                else
                {
                    //安装
                    report.Add(comment + "安装...");
                    flag = Install(stnm, tcode, ip);//安装
                }

                //部署后自动开启
                if (AutoStart)
                {
                    DirectoryInfo dir = GetDir_FromDeploy(name);
                    if (dir != null)
                    {
                        FileInfo fi_start = new FileInfo(dir.FullName + "\\Start.bat");
                        if (File.Exists(fi_start.FullName))
                        {
                            Process.Start(fi_start.FullName);
                            report.Add(name + "已开启");
                        }
                        else { report.Add(fi_start.FullName + "未找到"); }
                    }
                }
            }
            catch (Exception e)
            {
                report.Error("部署" + comment + "异常", e);
            }
            finally
            {
                report.Add("部署-" + comment, flag ? "成功" : "失败");
            }
        }
    }
}
