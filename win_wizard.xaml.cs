using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using TOEC_Dist.Model;
using Heng;

namespace TOEC_Dist
{
    /// <summary>
    /// win_wizard.xaml 的交互逻辑
    /// </summary>
    public partial class win_wizard : Window
    {
        public ObservableCollection<bind_progress> src_report { get { return report.src; } }
        public ObservableCollection<software> src_template { get { return template.list_t; } set { template.list_t = value; } }

        public win_wizard()
        {
            InitializeComponent();

            //程序集版本
            tbk_ver.Content += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //初始化-车站信息（异步加载）
            Task.Factory.StartNew(() => { Init_stinfo(); });
            //初始化-加载模板信息
            if (!template.Init_LoadTemplate()) { this.Close(); }
            //焦点在“预安装”，用于提醒
            btn_manual.Focus();
            //默认数据上下文
            DataContext = this;
        }

        #region 初始化-车站信息
        /// <summary>
        /// 用户输入：站名、电报码、ip
        /// </summary>
        public static string stnm, tcode, ip;
        /// <summary>
        /// 车站信息字典
        /// </summary>
        private static Dictionary<string, string> dic_st = new Dictionary<string, string>();
        /// <summary>
        /// 初始化：
        /// </summary>
        private void Init_stinfo()
        {
            FileInfo fi_stinfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config/", "stinfo.txt"));
            FileInfo fi_new = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config/", "new.txt"));
            if (File.Exists(fi_stinfo.FullName))
            {
                string[] lines = File.ReadAllLines(fi_stinfo.FullName, Encoding.UTF8);
                if (lines.Length > 0)
                {
                    foreach (string l in lines)
                    {
                        try
                        {
                            string[] nm_tcode = l.Split("#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (nm_tcode.Length == 2)
                            {
                                dic_st.Add(nm_tcode[0], nm_tcode[1]);
                            }
                        }
                        catch (Exception e)
                        {
                            report.Add("车站信息加载异常" + e, "失败");
                            continue;
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 站名与电报码 联动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_stnm_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt_stnm = (TextBox)sender;
            KeyValuePair<string, string> kv = dic_st.Where(n => n.Key == txt_stnm.Text).FirstOrDefault();
            txt_tcode.Text = kv.Value;
        }
        #endregion

        #region 事件-标题栏
        /// <summary>
        /// 窗口移动事件
        /// </summary>
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion 标题栏事件

        #region 事件-页面切换
        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tc_main.SelectedIndex += 1;
                btn_pre.IsEnabled = true;
                if (tc_main.SelectedIndex == tc_main.Items.Count - 1)
                {
                    btn_deploy.Visibility = Visibility.Visible;
                    btn_next.IsEnabled = false;
                }
            }
            catch { }
        }
        private void btn_Pre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tc_main.SelectedIndex > 0)
                {
                    tc_main.SelectedIndex -= 1;
                    btn_deploy.Visibility = Visibility.Hidden;
                    btn_next.IsEnabled = true;
                }
                else
                {
                    btn_pre.IsEnabled = false;
                }
            }
            catch { }
        }

        #endregion

        #region 事件-DataGrid（模板明细）选中
        private void dataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg != null)
            {
                software s = dg.SelectedItem as software;
                if (s != null)
                {
                    if (s.IsEnable == true)
                    {
                        s.IsEnable = false;
                    }
                    else
                    {
                        s.IsEnable = true;
                    }
                }
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            ItemCollection list = dg_template.Items;
            if (list != null && list.Count > 0)
            {
                foreach (software item in list)
                {
                    item.IsEnable = true;
                }
            }

        }

        /// <summary>
        /// 全不选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectedAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ItemCollection list = dg_template.Items;
            if (list != null && list.Count > 0)
            {
                foreach (software item in list)
                {
                    item.IsEnable = false;
                }
            }
        }
        #endregion

        #region 事件-按钮-打开 预安装文件夹
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_manual_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir_manual = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Config.GetAppConfig("path_manual")));
            if (Directory.Exists(dir_manual.FullName))
            {
                Process.Start(dir_manual.FullName);
            }
            else
            {
                Directory.CreateDirectory(dir_manual.FullName);
                Process.Start(dir_manual.FullName);
            }
        }
        #endregion

        #region 事件-按钮-部署
        /// <summary>
        /// 一键部署
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_deploy_Click(object sender, RoutedEventArgs e)
        {
            stnm = txt_stnm.Text.Trim();
            tcode = txt_tcode.Text.Trim();
            ip = txt_ip.Text.Trim();
            if (string.IsNullOrWhiteSpace(stnm) ||
                string.IsNullOrWhiteSpace(tcode) ||
                string.IsNullOrWhiteSpace(ip)) { MessageBox.Show("车站基本信息不能省略"); return; }
            try
            {
                //执行部署
                btn_deploy.IsEnabled = false;
                report.Clear();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        software.CloseDaemon();//关闭守护进程
                        software.Delete_Old_Software();//清除老版本
                        template.DeployAll(stnm, tcode, ip);//部署
                        software.OpenDaemon();//开启闭守护进程
                    }
                    catch (Exception ex) { report.Add(ex.Message, "异常"); }
                    finally { btn_deploy.Dispatcher.Invoke(new Action(() => { btn_deploy.IsEnabled = true; })); }
                });

            }
            catch (Exception ex) { report.Error("", ex); }
        }
        #endregion
    }
}
