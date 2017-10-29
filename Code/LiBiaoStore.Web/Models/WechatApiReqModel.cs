using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiBiaoStore.Web.Models
{
    public class WechatApiReqModel
    {
        public string signature { set; get; }
        public string msg_signature { set; get; }
        public string timestamp { set; get; }
        public string nonce { set; get; }
        public string echostr { set; get; }
    }
}