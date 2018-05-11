using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heng;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows.Threading;
using System.Threading;
using System.Data;

namespace TOEC_Dist.Model
{
    /// <summary>
    /// 数据库部署
    /// 注意：
    /// 该部署方式相对僵化：只适用于免安装版本的mysql数据库
    /// </summary>
    public class database : software
    {
        /// <summary>
        /// 脚本压缩包名称
        /// </summary>
        public string name_sql { get; set; }

        public string con { get { return Config.GetAppConfig("db_con"); } }
        /// <summary>
        /// 部署
        /// </summary>
        /// <returns></returns>
        public override void Deploy(string stnm, string tcode, string ip)
        {
            bool flag = false;
            try
            {
                //服务是否存在
                if (Helper_Service.Service_IsExisted(name))
                {
                    report.Add(comment + "升级...");
                    flag = Update();//升级
                }
                else
                {
                    report.Add(comment + "安装...");
                    flag = Install_DB(stnm, tcode, ip);//安装


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

        #region 安装
        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="StationName">站名</param>
        /// <param name="TelexCode">电报码</param>
        /// <param name="IP">IP</param>
        /// <returns></returns>
        private bool Install_DB(string StationName, string TelexCode, string IP)
        {
            try
            {
                //安装
                if (!Install(StationName, TelexCode, IP)) { return false; }

                //获取具体部署目录
                DirectoryInfo dir_deploy_db = GetDir_FromDeploy(name);

                //CMD方式安装
                report.Add("安装...");
                List<string> cmds = new List<string>();
                cmds.Add(@"cd " + dir_deploy_db.FullName + @"\bin\");
                cmds.Add(dir_deploy.FullName.Substring(0, 1) + ":");//切换盘符
                cmds.Add("mysqld install mysql");
                cmds.Add("net start mysql");
                cmds.Add("exit");
                Common_Handle.RunCMD(cmds);

                #region 初始化-默认数据
                report.Add("默认数据初始化...");
                try
                {
                    //数据库默认数据赋值
                    List<string> list_sql = new List<string>();
                    list_sql.Add("UPDATE line l SET l.TelexCode='" + TelexCode + "' , l.NextTelexCode='';");
                    list_sql.Add("UPDATE sys_CodeMap p SET `Value`='" + StationName + "' where p.`Code`='StationName';");
                    list_sql.Add("UPDATE sys_CodeMap p SET `Value`='" + TelexCode + "' where p.`Code`='TelexCode';");
                    list_sql.Add("UPDATE sys_CodeMap p SET `Value`='" + IP + "' where p.`Code`='IP';");
                    list_sql.Add("UPDATE account_t_station s SET s.StationName='" + StationName + "', s.ConnectString='" + IP + "', s.TelexCode='" + TelexCode + "', s.StationType='0' WHERE s.StationID='100'; ");
                    list_sql.Add("UPDATE account_t_user u SET u.TelexCode='" + TelexCode + "' WHERE u.userid='1';");
                    foreach (string s in list_sql)
                    {
                        string error = ExecSQL(s, con);
                        if (error != "")
                        {
                            report.Add("[执行" + s + "失败]" + error, "失败");
                        }
                    }
                    report.Add("数据库安装完成", "成功");
                }
                catch { throw; }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                report.Error("数据库安装异常", e);
                return false;
            }
        }
        #endregion

        #region 升级
        /// <summary>
        /// 升级
        /// </summary>
        /// <returns></returns>
        private bool Update()
        {
            try
            {
                //检查服务是否正常
                if (Helper_Service.Server_IsOpen(name))
                {
                    report.Add("检测到数据库服务正常，准备执行脚本");
                }
                else
                {
                    report.Add("检测到数据库服务未启动，尝试启动");
                    if (Helper_Service.Server_Open(name))
                    {
                        report.Add("启动成功");
                    }
                    else { report.Error("启动失败"); return false; }
                }
                //备份
                if (!Backup(name)) return false;

                //找到压缩包
                FileInfo zip = GetZipFile_FromSource(name_sql);
                if (zip == null) return false;

                //解压缩 至 源目录
                report.Add("解压缩...");
                if (Helper_Zip.UnZip(zip.FullName, dir_source.FullName, "", true))
                {
                    report.Add("脚本解压成功");
                }
                else
                {
                    report.Error("脚本解压失败"); return false;
                }

                //执行脚本
                DirectoryInfo SQLDir = new DirectoryInfo(zip.FullName.TrimEnd(".zip".ToCharArray()));
                if (Directory.Exists(SQLDir.FullName))
                {
                    //获去当前数据库版本号
                    string dbv = GetDBVersion(con);
                    if (!string.IsNullOrWhiteSpace(dbv))
                    {
                        report.Add("数据库当前版本：" + dbv);
                        foreach (FileInfo s in SQLDir.GetFiles("*.sql"))
                        {
                            try
                            {
                                string BatVersion = Path.GetFileNameWithoutExtension(s.FullName).ToUpper().Replace("UPDATE_", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                                if (long.Parse(BatVersion) > long.Parse(dbv))
                                {
                                    string ErrorInfo = ExecSQLFile(s.FullName, con);
                                    if (ErrorInfo != "")
                                    {
                                        report.Error(s.Name + "【" + ErrorInfo + "】");
                                    }
                                    else
                                    {
                                        report.Add(s.Name, "成功");
                                    }
                                }
                            }
                            catch (Exception ex) { report.Error("【执行异常】" + s.Name, ex); continue; }
                        }
                    }
                    else { report.Error("无法获取当前数据库版本"); }
                }
                return true;
            }
            catch (Exception e)
            {
                report.Error("数据库升级异常", e);
                return false;
            }
        }
        #endregion

        #region 数据库操作
        private static string ExecSQL(string SQLCommand, string ConStr)
        {
            if (string.IsNullOrWhiteSpace(SQLCommand)) { return ""; }
            if (string.IsNullOrWhiteSpace(ConStr)) { return "数据库连接字符串未配置"; }
            try
            {
                using (MySqlConnection mycon = new MySqlConnection(ConStr))
                {
                    mycon.Open();
                    MySqlCommand cmd = new MySqlCommand(SQLCommand, mycon);
                    cmd.CommandTimeout = 60;
                    int rowCount = cmd.ExecuteNonQuery();
                    mycon.Close();
                }
            }
            catch (Exception ex)
            {
                return "异常:" + ex.Message;
            }
            return "";
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        private static string ExecSQLFile(string filename, string ConStr)
        {
            if (!File.Exists(filename)) { return "文件不存在"; }
            if (string.IsNullOrWhiteSpace(ConStr)) { return "数据库连接字符串未配置"; }
            string[] filetemp = filename.Split('\\');
            try
            {
                string TmpSQL = "";
                try
                {
                    TmpSQL = Helper_FileDir.ReadFile(filename);
                }
                catch (Exception ex) { return "执行" + filetemp[filetemp.Length - 1] + "失败" + ex.Message; }

                using (MySqlConnection mycon = new MySqlConnection(ConStr))
                {
                    mycon.Open();
                    MySqlCommand cmd = new MySqlCommand(TmpSQL, mycon);
                    cmd.CommandTimeout = 60;
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount > 0)
                    {
                        //Component_Handle.Log_Lv(lv_Log, rowCount + "行，受影响", "", "成功", CommonColor.Success);
                    }
                    //插入版本
                    string[] nameArray = filetemp[filetemp.Length - 1].ToLower().Replace(".sql", "").Replace(".txt", "").Replace("update_", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string new_version = "", Remark = "";
                    new_version = nameArray[0];//记录版本号
                    if (nameArray.Length > 1)
                    {
                        Remark = nameArray[1];//有中文注释则记录
                    }
                    MySqlCommand version_Cmd = new MySqlCommand("insert dbversion values('0','" + new_version + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Remark + "')", mycon);
                    version_Cmd.ExecuteNonQuery();
                    mycon.Close();
                }
            }
            catch (Exception ex)
            {
                return "执行" + filetemp[filetemp.Length - 1] + "失败" + ex.Message;
            }
            return "";
        }
        private static string GetDBVersion(string ConStr)
        {
            string strdbv = "";
            string strsql = "SELECT 1 num FROM DBVersion ";
            DataTable dt = QueryTable(strsql, ConStr);
            if (dt.Rows.Count > 0)
            {
                strsql = "select * from DBVersion order by db_version DESC LIMIT 1;";
                DataTable dt1 = QueryTable(strsql, ConStr);
                if (dt1.Rows.Count > 0)
                {
                    strdbv = dt1.Rows[0]["DB_Version"].ToString();
                }
            }
            return strdbv;
        }
        private static DataTable QueryTable(string sqlstr, string ConStr)
        {
            if (sqlstr == null || sqlstr == "")
                throw new Exception();
            DataTable datatable = new DataTable();
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection mycon = new MySqlConnection(ConStr))
            {
                try
                {
                    mycon.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlstr;
                    cmd.CommandTimeout = 600;
                    cmd.Connection = mycon;
                    MySqlDataAdapter mda = new MySqlDataAdapter(cmd);
                    mda.Fill(datatable);
                    return datatable;
                }
                catch { return datatable; }
                finally { mycon.Close(); }
            }
        }
        #endregion
    }
}
