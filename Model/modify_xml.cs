using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOEC_Dist.Model
{
    /// <summary>
    /// 修改配置文件：xml
    /// </summary>
    public class modify_xml
    {
        public modify_xml(string p, string i, string a, string v, string inner, string e)
        {
            path = p;
            id = i;
            attr = a;
            value = v;
            innerText = inner;
            encode = e;
        }
        public string path { get; set; }
        public string id { get; set; }
        public string attr { get; set; }
        public string value { get; set; }
        public string innerText { get; set; }
        public string encode { get; set; }
    }
}
