using Heng;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TOEC_Dist.Model
{
    /// <summary>
    /// 基类：软件
    /// 子类包含：服务、软件、网站、数据库、共享文件夹
    /// </summary>
    public class software : INotifyPropertyChanged
    {
        public software()
        {
            list_mx = new List<modify_xml>();
        }
        /// <summary>
        /// 所有软件的部署根目录
        /// </summary>
        public static DirectoryInfo dir_deploy
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Config.GetAppConfig("path_deploy")));
                if (!Directory.Exists(dir.FullName)) { Directory.CreateDirectory(dir.FullName); }
                return dir;
            }
        }

        /// <summary>
        /// 备份路径
        /// </summary>
        public static DirectoryInfo dir_backup
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Config.GetAppConfig("path_backup")));
                if (!Directory.Exists(dir.FullName)) { Directory.CreateDirectory(dir.FullName); }
                return dir;
            }
        }

        public static DirectoryInfo dir_source
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Config.GetAppConfig("path_source")));
                if (!Directory.Exists(dir.FullName)) { Directory.CreateDirectory(dir.FullName); }
                return dir;
            }
        }

        /// <summary>
        /// 修改配置文件:xml
        /// </summary>
        public List<modify_xml> list_mx { get; set; }
        /// <summary>
        /// 软件名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 软件说明
        /// </summary>
        public string comment { get; set; }

        private bool _IsEnable;
        /// <summary>
        /// 是否部署
        /// </summary>
        public bool IsEnable { get { return _IsEnable; } set { _IsEnable = value; GetChanged("IsEnable"); } }

        /// <summary>
        /// 部署后是否自启
        /// </summary>
        public bool AutoStart { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void GetChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }


        #region 获取-压缩包文件
        /// <summary>
        /// 获取压缩包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected FileInfo GetZipFile_FromSource(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (FileInfo fi in dir_source.GetFiles("*.zip"))
                {
                    if (fi.Name.Replace(fi.Extension, "").ToLower() == name.ToLower())
                    {
                        return fi;
                    }
                }
            }
            report.Error("未找到压缩包" + name);
            return null;
        }
        #endregion 获取压缩包

        #region 获取-安装目录
        /// <summary>
        /// 获取安装目录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static DirectoryInfo GetDir_FromDeploy(string name, bool AllowLog = true)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (DirectoryInfo d in dir_deploy.GetDirectories())
                {
                    if (d.Name.ToLower() == name.ToLower())
                    {
                        return d;
                    }
                }
            }
            if (AllowLog) { report.Error("未找到安装目录" + name); }
            return null;
        }
        #endregion 获取压缩包

        #region 获取-安装目录的特定的文件（默认为exe）
        /// <summary>
        /// 获取安装目录的特定的文件（默认为exe）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extendtion"></param>
        /// <returns></returns>
        protected static FileInfo GetFile_FromDeploy(string name, string extendtion = "exe")
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                //遍历部署目录
                DirectoryInfo d = GetDir_FromDeploy(name, false);
                if (d != null)
                {
                    //遍历所有exe文件
                    foreach (FileInfo exe in d.GetFiles("*." + extendtion))
                    {
                        if (exe.Name.Replace(".exe", "").ToLower() == name.ToLower())
                        {
                            return exe;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        #endregion

        #region 部署
        /// <summary>
        /// 部署
        /// 虚方法：根据具体软件类型进行实现
        /// </summary>
        public virtual void Deploy(string stnm = "", string tcode = "", string ip = "")
        {
            //根据具体软件类型进行实现
        }
        #endregion

        #region 安装
        /// <summary>
        /// 安装软件
        /// 流程：解压 修改配置文件
        /// 适用：数据库\服务\应用程序\网络站点
        /// </summary>
        /// <param name="stnm"></param>
        /// <param name="tcode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected bool Install(string stnm, string tcode, string ip)
        {
            try
            {
                //解压
                report.Add("解压缩...");
                FileInfo fi_zip = GetZipFile_FromSource(name);
                if (fi_zip == null) return false;
                Helper_Zip.UnZip(fi_zip.FullName, dir_deploy.FullName, "", true);

                //获取具体软件部署目录
                DirectoryInfo dir_soft = GetDir_FromDeploy(name);

                //修改配置文件
                if (list_mx.Count > 0)
                {
                    report.Add("修改配置文件...");
                    foreach (modify_xml mx in list_mx)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(mx.innerText))
                            {
                                Helper_XML.ModifyAttr_ByID(dir_soft.FullName + "\\" + mx.path,
                                    mx.id,
                                    mx.attr,
                                    mx.value.Replace("@TelexCode", tcode).Replace("@IP", ip),
                                    mx.encode);
                                report.Add("【" + mx.path + "】id:" + mx.id + "的属性" + mx.attr + "置为" + mx.value.Replace("@TelexCode", tcode).Replace("@IP", ip), "成功");
                            }
                            else
                            {
                                Helper_XML.ModifyText_ByID(dir_soft.FullName + "\\" + mx.path,
                                    mx.id,
                                    mx.innerText.Replace("@TelexCode", tcode).Replace("@IP", ip),
                                    mx.encode);
                                report.Add("【" + mx.path + "】id:" + mx.id + "的InnerText置为" + mx.innerText.Replace("@TelexCode", tcode).Replace("@IP", ip), "成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            report.Error(mx.path, ex);
                            continue;
                        }
                    }
                }

                //获取新版本号
                FileInfo exe_new = GetFile_FromDeploy(name);
                if (exe_new != null) { report.Add("新版本：" + Common_Handle.GetExeVersion(exe_new.FullName)); }

                return true;
            }
            catch (Exception e)
            {
                report.Error(comment + "安装[解压+修改配置文件]异常", e);
                return false;
            }
        }
        #endregion

        #region 升级
        /// <summary>
        /// 升级
        /// 流程：备份+获取版本号+（解压+修改配置文件）+获取新版本号
        /// 适用：服务\应用程序\网络站点
        /// </summary>
        /// <param name="stnm"></param>
        /// <param name="tcode"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected bool Update(string stnm, string tcode, string ip)
        {
            try
            {
                //备份
                if (!Backup(name)) return false;

                //获取旧版本号
                FileInfo exe_old = GetFile_FromDeploy(name);
                if (exe_old != null) { report.Add("旧版本：" + Common_Handle.GetExeVersion(exe_old.FullName)); }

                //安装（解压+修改配置文件）
                if (!Install(stnm, tcode, ip)) { return false; }

                return true;
            }
            catch (Exception e)
            {
                report.Error(name + "升级异常", e);
                return false;
            }

        }
        #endregion

        #region 备份
        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Backup(string name)
        {
            try
            {
                bool GoOn = bool.Parse(Config.GetAppConfig("enable_backup"));
                if (!GoOn)
                {
                    report.Add("已经禁用备份功能"); return true;
                }
                DirectoryInfo dir = GetDir_FromDeploy(name, false);
                if (dir != null)
                {
                    report.Add("开始备份...");
                    //删除原备份
                    foreach (DirectoryInfo old_backup in dir_backup.GetDirectories())
                    {
                        if (old_backup.Name.ToLower() == name.ToLower())
                        {
                            Directory.Delete(old_backup.FullName, true);
                            report.Add("删除了原有备份");
                            break;
                        }
                    }

                    //复制
                    try
                    {
                        Helper_FileDir.CopyDirectory(dir.FullName, dir_backup.FullName);
                        report.Add("备份成功");
                    }
                    catch (Exception e) { report.Error("备份拷贝异常", e); }
                    return true;
                }
                else
                {
                    report.Error("无法备份：未找到" + name);
                    return false;
                }
            }
            catch (Exception e)
            {
                report.Error("备份异常", e);
                return false;
            }
        }
        #endregion 备份

        #region 关闭守护进程
        /// <summary>
        /// 关闭守护进程
        /// </summary>
        /// <returns></returns>
        public static bool CloseDaemon()
        {
            if (Helper_Service.Service_IsExisted("TOEC_Daemon"))
            {
                if (Helper_Service.Server_Close("TOEC_Daemon"))
                {
                    report.Add("守护进程已经停止");
                    return true;
                }
                else
                {
                    report.Error("守护进程关闭异常");
                    return false;
                }
            }
            else
            {
                report.Add("未安装守护进程");
                return true;
            }
        }
        #endregion 关闭守护进程

        #region 开启守护进程
        public static void OpenDaemon()
        {
            DirectoryInfo dir = GetDir_FromDeploy("TOEC_Daemon", false);
            if (dir != null)
            {
                FileInfo fi = new FileInfo(dir.FullName + "\\Start.bat");
                if (File.Exists(fi.FullName))
                {
                    if (!Helper_Service.Server_IsOpen("TOEC_Daemon"))
                    {
                        Process p = Process.Start(fi.FullName);
                        p.WaitForExit(15000);
                        report.Add("守护进程已经开启", "成功");
                    }
                    else { report.Add("守护进程已经开启", "成功"); }
                }
                else
                {
                    report.Error("未找到守护进程服务的Start.bat");
                }
            }
            else { report.Add("未安装守护进程"); }
        }
        #endregion 开启守护进程

        #region 删除老旧服务
        /// <summary>
        /// 删除老旧服务
        /// 特殊处理老旧的服务
        /// </summary>
        public static bool Delete_Old_Software()
        {
            string[] old_name = Config.GetAppConfig("del_process").Split(',');

            foreach (var old in old_name)
            {
                if (Helper_Process.CheckProcessExists(old))//检测进程
                {
                    report.Add("检测到老旧版本【" + old + "】");
                    FileInfo fi = new FileInfo(Process.GetProcessesByName(old)[0].MainModule.FileName);//通过进程获取文件位置
                    DirectoryInfo dir = new DirectoryInfo(fi.DirectoryName);
                    FileInfo stop = new FileInfo(dir.FullName + "\\Stop.bat");
                    if (File.Exists(stop.FullName))
                    {
                        try
                        {
                            Process p = Process.Start(stop.FullName);
                            p.WaitForExit(15000);
                        }
                        catch (Exception ex)
                        {
                            report.Error("卸载服务异常" + old, ex); return false;
                        }
                    }
                    else
                    {
                        report.Error("未找到老服务Stop.bat文件");
                    }
                    Thread.Sleep(5000);

                    //检查进程：进程名称与exe名称一致
                    if (Helper_Process.CheckProcessExists(old))
                    {
                        if (Helper_Process.KillProcess(old))
                        {
                            report.Add("进程已经被强制结束");
                        }
                        else
                        {
                            report.Error("进程强制结束失败"); return false;
                        }
                    }
                    dir.Delete(true);
                    report.Add("【" + old + "】已卸载", "成功");
                }
            }
            return true;
        }
        #endregion
    }
}
