using Heng;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using TOEC_Dist.Model;

namespace TOEC_Dist.Model
{
    /// <summary>
    /// 模板类
    /// </summary>
    public static class template
    {
        /// <summary>
        /// 模板文档
        /// </summary>
        public static XmlDocument doc = new XmlDocument();

        /// <summary>
        /// 模板软件加载后的存储列表
        /// </summary>
        public static ObservableCollection<software> list_t = new ObservableCollection<software>();

        #region 加载模板 将模板中的节点转为实体对象
        /// <summary>
        /// 加载模板
        /// 将模板中的节点转为实体对象
        /// </summary>
        public static bool Init_LoadTemplate()
        {
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Config.GetAppConfig("path_template")));
                if (!File.Exists(fi.FullName))
                {
                    MessageBox.Show("缺失部署模板", "警告", MessageBoxButton.OK, MessageBoxImage.Stop); return false;
                }

                doc.Load(fi.FullName);

                #region 加载-文件夹
                XmlNodeList xml_share = doc.GetElementsByTagName("folder");
                foreach (XmlNode n in xml_share)
                {
                    folder f = new folder();
                    f.comment = n.GetAttr("comment");
                    f.name = n.GetAttr("name");
                    f.path = n.GetAttr("path");
                    f.IsEnable = bool.Parse(n.GetAttr("enable"));
                    foreach (XmlNode sub in n.ChildNodes)
                    {
                        if (sub.Name == "sub")
                        {
                            f.subfolder.Add(sub.InnerText);
                        }
                    }
                    list_t.Add(f);
                }
                #endregion

                #region 加载-数据库
                XmlNodeList xml_db = doc.GetElementsByTagName("db");
                foreach (XmlNode n in xml_db)
                {
                    database db = new database();
                    db.name = n.GetAttr("name");
                    db.name_sql = n.GetAttr("sql");
                    db.comment = n.GetAttr("comment");
                    db.IsEnable = bool.Parse(n.GetAttr("enable"));
                    list_t.Add(db);
                }
                #endregion

                #region 加载-应用程序  
                XmlNodeList xml_app = doc.GetElementsByTagName("app");
                foreach (XmlNode s in xml_app)
                {
                    appliction app = new appliction();
                    app.name = s.GetAttr("name");
                    app.comment = s.GetAttr("comment");
                    app.AutoStart = bool.Parse(s.GetAttr("autostart"));
                    app.IsEnable = bool.Parse(s.GetAttr("enable"));
                    foreach (XmlNode m in s.ChildNodes)//读取配置修改明细
                    {
                        if (m.Name == "xml")
                        {
                            app.list_mx.Add(new modify_xml(
                                m.GetAttr("path"),
                                m.GetAttr("id"),
                                m.GetAttr("attr"),
                                m.GetAttr("value"),
                                m.InnerText,
                                m.GetAttr("encode")
                                ));
                        }
                    }
                    list_t.Add(app);
                }
                #endregion

                #region 加载-系统服务
                XmlNodeList xml_srv = doc.GetElementsByTagName("srv");
                foreach (XmlNode n in xml_srv)
                {
                    server srv = new server();
                    srv.name = n.GetAttr("name");
                    srv.comment = n.GetAttr("comment");
                    srv.AutoStart = bool.Parse(n.GetAttr("autostart"));
                    srv.IsEnable = bool.Parse(n.GetAttr("enable"));
                    foreach (XmlNode m in n.ChildNodes)//读取配置修改明细
                    {
                        if (m.Name == "xml")
                        {
                            srv.list_mx.Add(new modify_xml(
                                m.GetAttr("path"),
                                m.GetAttr("id"),
                                m.GetAttr("attr"),
                                m.GetAttr("value"),
                                m.InnerText,
                                m.GetAttr("encode")
                                ));
                        }
                    }
                    list_t.Add(srv);
                }
                #endregion

                #region 加载-网络服务
                XmlNodeList xml_web = doc.GetElementsByTagName("web");
                foreach (XmlNode s in xml_web)
                {
                    web web = new web();
                    web.name = s.GetAttr("name");
                    web.comment = s.GetAttr("comment");
                    web.port = int.Parse(s.GetAttr("port"));
                    web.AutoStart = bool.Parse(s.GetAttr("autostart"));
                    web.IsEnable = bool.Parse(s.GetAttr("enable"));
                    foreach (XmlNode m in s.ChildNodes)//读取虚拟目录配置
                    {
                        if (m.Name == "vf")
                        {
                            web.list_vf.Add(new KeyValuePair<string, string>(m.GetAttr("name"), m.InnerText));
                        }
                    }
                    list_t.Add(web);
                }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("【加载模板异常】" + e.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 根据实体对象列表 进行部署
        /// </summary>
        /// <param name="stnm"></param>
        /// <param name="tcode"></param>
        /// <param name="ip"></param>
        public static void DeployAll(string stnm = "", string tcode = "", string ip = "")
        {
            Deploy(list_t, stnm, tcode, ip);
        }

        /// <summary>
        /// 部署某一类软件
        /// 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list_t"></param>
        /// <param name="stnm"></param>
        /// <param name="tcode"></param>
        /// <param name="ip"></param>
        private static void Deploy<T>(ObservableCollection<T> list_t, string stnm = "", string tcode = "", string ip = "") where T : software
        {
            foreach (T t in list_t)
            {
                if (!t.IsEnable) { report.Add("部署-" + t.comment, "跳过"); continue; }
                t.Deploy(stnm, tcode, ip);
            }
        }
    }
}
