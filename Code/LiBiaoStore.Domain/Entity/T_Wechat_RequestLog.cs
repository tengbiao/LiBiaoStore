using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiBiaoStore.Domain.Entity
{
    public class T_Wechat_RequestLog
    {
        public string ID { set; get; }
        public string WechatAdminId { set; get; }
        public string MsgId { set; get; }
        public string FromUserName { set; get; }
        public string FromCreateTime { set; get; }
        public string MsgType { set; get; }
        public string Content { set; get; }
        public string CreateUser { set; get; }
        public DateTime? CreateTime { set; get; }
        public string UpdateUser { set; get; }
        public DateTime? UpdateTime { set; get; }
    }
}
