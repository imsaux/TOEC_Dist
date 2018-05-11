using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TOEC_Dist
{
    /// <summary>
    /// 安装进度日志
    /// </summary>
    public static class report
    {
        static report()
        {
            src = new ObservableCollection<bind_progress>();
        }

        //【进度信息】被绑定的数据源
        public static ObservableCollection<bind_progress> src { get; set; }

        public static void Add(string i, string s = "")
        {
            //使用主线程调度去更新数据源
            Application.Current.Dispatcher.Invoke(new Action(() => { src.Add(new bind_progress(i, s)); }));
        }

        public static void Error(string i, Exception e = null)
        {
            string msg = "";
            if (e != null) msg = "【" + e.Message + "】";
            //使用主线程调度去更新数据源
            Application.Current.Dispatcher.Invoke(new Action(() => { src.Add(new bind_progress(i + msg, "异常")); }));
        }

        public static void Clear()
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { src.Clear(); }));
        }
    }
}
