using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LiBiaoStore.Web.Helper
{
    public class HandlerWebChatMessage
    {
        /// <summary>
        /// 处理客户发送过来的数据
        /// </summary>
        /// <param name="msg"></param>
        public static void StartHanlder(Dictionary<string, string> msg, out string returnMessage)
        {
            returnMessage = string.Empty;
            var msgtype = msg["MsgType"];
            var touser = msg["ToUserName"];
            var openid = msg["FromUserName"];
            restart:
            switch (msgtype)
            {
                case MessageType.TEXT:
                    returnMessage = HandlerText(touser, openid, msg);
                    break;
                case MessageType.VOICE:
                    returnMessage = HandlerVoice(touser, openid, msg);
                    break;
                case MessageType.LOCATION:
                    var location = msg["Label"];
                    returnMessage = GetTextMessage(openid, touser, "[阴险]你现在在" + location + "对不对？小心我晚上来找你！");
                    break;
                case MessageType.IMAGE:
                    HandlerImage(touser, openid, msg);
                    //发送评论
                    var pl = new { touser = openid, msgtype = "text", text = new { content = "这张照片拍的真好，快点告诉我里面有没有美女[色]" } };
                    //MpHelper.SendMessage(pl);
                    break;
                case MessageType.EVENT:
                    msgtype = MessageType.Default;
                    goto restart;//事件处理暂时跳转到默认发送
                default:
                    returnMessage = HandlerDefault(touser, openid, msg);
                    break;
            }
        }
        //处理文本
        private static string HandlerText(string touser, string openid, Dictionary<string, string> data)
        {
            string result = string.Empty;
            string text = data["Content"];
            if (text == "1")
            {
                result = GetTextMessage(openid, touser, "老婆，老婆，我爱你[嘴唇][嘴唇][嘴唇]");
            }
            else if (text == "2")
            {
                result = GetTextMessage(openid, touser, "不要问我猪是怎么死的。[猪头]");
            }
            else if (text == "3")
            {
                result = GetTextMessage(openid, touser, "您说什么，我是个天才，哈哈哈哈哈[憨笑]");
            }
            else if (text.Length > 5)
            {
                result = GetTextMessage(openid, touser, text);
            }
            else
            {
                result = GetTextMessage(openid, touser, "你个傻X，不知道你在说什么[咒骂][咒骂][咒骂][咒骂]");
            }
            return result;
        }

        //回复声音 暂时发送什么回复什么
        private static string HandlerVoice(string touser, string openid, Dictionary<string, string> data)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[voice]]></MsgType>
                                    <Voice>
                                    <MediaId><![CDATA[{3}]]></MediaId>
                                    </Voice>
                                 </xml>", openid, touser, DateTime.Now.Ticks, data["MediaId"]);
            return result.ToString();
        }

        //回复图片 暂时发送什么回复什么
        private static string HandlerImage(string touser, string openid, Dictionary<string, string> data)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[image]]></MsgType>
                                    <Image>
                                    <MediaId><![CDATA[{3}]]></MediaId>
                                    </Image>
                                  </xml>", openid, touser, DateTime.Now.Ticks, data["MediaId"]);
            return result.ToString();
        }


        //默认处理， 发送选择序号提示
        private static string HandlerDefault(string touser, string openid, Dictionary<string, string> data)
        {
            //var user = MpHelper.GetUserInfo(openid);
            string username = "外星人";
            //if (user != null)
            //{
            //    //username = user["province"].ToString() + user["city"] + "的" + user["nickname"];
            //}
            return GetTextMessage(openid, touser, string.Format("来自{0}您好[愉快]\r回复序号\r[1]:有惊喜\r[2]:告诉您个秘密\r[3]:嘿嘿", username));
        }

        //根据参数获得文本消息
        private static string GetTextMessage(string openid, string wechatid, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<xml>
                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                <CreateTime>{2}</CreateTime>
                                <MsgType><![CDATA[text]]></MsgType>
                                <Content><![CDATA[{3}]]></Content>
                            </xml>", openid, wechatid, DateTime.Now.Ticks, msg);
            return sb.ToString();
        }
    }

    public class MessageType
    {
        /// <summary>
        /// 默认
        /// </summary>
        public const string Default = "default";
        /// <summary>
        /// 文本
        /// </summary>
        public const string TEXT = "text";
        /// <summary>
        /// 语音
        /// </summary>
        public const string VOICE = "voice";
        /// <summary>
        /// 地址
        /// </summary>
        public const string LOCATION = "location";
        /// <summary>
        /// 事件
        /// </summary>
        public const string EVENT = "event";
        /// <summary>
        /// 图片
        /// </summary>
        public const string IMAGE = "image";
    }
}