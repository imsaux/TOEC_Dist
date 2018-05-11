using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOEC_Dist
{
    /// <summary>
    /// 部署进度信息
    /// 日志、进度条
    /// </summary>
    public class bind_progress : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bind_progress(string i = "", string s = "")
        {
            info = i;
            status = s;
        }

        /// <summary>
        /// 进度信息
        /// </summary>
        private string _info;
        public string info { get { return _info; } set { _info = value; GetChanged("info"); } }
      
        /// <summary>
        /// 结果状态
        /// </summary>
        private string _status;
        public string status { get { return _status; } set { _status = value; GetChanged("status"); } }

        /// <summary>
        /// 进度
        /// </summary>
        private static int _index;
        public static int index { get { return _index; } set { _index = value; } }

        private void GetChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}
