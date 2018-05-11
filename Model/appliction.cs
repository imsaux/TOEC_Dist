using Heng;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TOEC_Dist.Model
{
    public class appliction : software
    {
        public override void Deploy(string stnm, string tcode, string ip)
        {
            bool flag = false;
            try
            {
                //进程是否存在
                if (Helper_Process.CheckProcessExists(name) || GetDir_FromDeploy(name, false) != null)
                {
                    //升级
                    report.Add(comment + "升级...");
                    //关闭程序
                    if (!Helper_Process.KillProcess(name))
                    {
                        report.Error(name + "程序关闭失败");
                    }
                    else
                    {
                        report.Add(name + "程序已关闭");
                        //开始升级
                        flag = Update(stnm, tcode, ip);
                    }
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
                    FileInfo fi_start = GetFile_FromDeploy(name);
                    if (fi_start != null && File.Exists(fi_start.FullName))
                    {
                        Process.Start(fi_start.FullName);
                        report.Add(name + "已开启");
                    }
                    else { report.Add(fi_start.FullName + "未找到"); }
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
