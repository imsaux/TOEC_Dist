using Heng;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOEC_Dist.Model
{
    public class web : software
    {
        /// <summary>
        /// 端口
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 虚拟目录集合
        /// </summary>
        public List<KeyValuePair<string, string>> list_vf = new List<KeyValuePair<string, string>>();
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
                //查询网络站点
                bool WebIsExist = false;
                ServerManager sm = new ServerManager();
                for (int i = 0; i < sm.Sites.Count; i++)
                {
                    if (sm.Sites[i].Name.ToLower() == name.ToLower())
                    {
                        WebIsExist = true; break;
                    }
                }
                //判断站点是否存在
                if (WebIsExist)
                {
                    report.Add(comment + "升级...");
                    flag = Update(stnm, tcode, ip);//升级
                }
                else
                {
                    report.Add(comment + "安装...");
                    //通用安装
                    if (!Install(stnm, tcode, ip)) { return; }

                    try
                    {
                        report.Add("创建程序池...");
                        if (sm.ApplicationPools[name] == null)
                        {
                            sm.ApplicationPools.Add(name);
                            ApplicationPool curAppPool = sm.ApplicationPools[name];
                            curAppPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                            curAppPool.Failure.RapidFailProtection = true;
                            curAppPool.Enable32BitAppOnWin64 = true;
                            sm.ApplicationPools[name].ManagedRuntimeVersion = "v4.0";//必须这么写才能选中默认V4.0.30319
                            curAppPool.AutoStart = true;
                        }
                        report.Add("创建站点...");
                        DirectoryInfo dir = GetDir_FromDeploy(name);

                        Site curweb = sm.Sites.Add(name, "http", ip + ":" + port + ":", dir.FullName);
                        //绑定默认IP
                        curweb.Bindings.Add("202.202.202.1:" + port + ":", "http");
                        curweb.Applications[0].ApplicationPoolName = name;
                        //开启站点
                        if (AutoStart) curweb.ServerAutoStart = true;

                        report.Add("创建虚拟目录...");
                        foreach (KeyValuePair<string, string> vf in list_vf)
                        {
                            curweb.Applications[0].VirtualDirectories.Add("/" + vf.Key, vf.Value);
                        }

                        report.Add("设置权限...");
                        Helper_FileDir.AddSecurityControll2Folder(dir.FullName);
                        sm.CommitChanges();

                        if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319"))
                        {
                            report.Add("注册IIS...");
                            List<string> cmds = new List<string>();
                            cmds.Add(@"cd C:\Windows\Microsoft.NET\Framework\v4.0.30319");
                            cmds.Add("C:");//切换盘符
                            cmds.Add("aspnet_regiis.exe -i");
                            cmds.Add("exit");
                            Common_Handle.RunCMD(cmds);
                        }
                        flag = true;
                    }
                    catch (Exception e) { report.Error("站点搭建异常", e); }
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
