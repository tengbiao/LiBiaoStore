using LiBiaoStore.Application.IApplication;
using LiBiaoStore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LiBiaoStore.Code;
using LiBiaoStore.Web.Helper;
using System.Xml;

namespace LiBiaoStore.Web.Controllers
{
    /// <summary>
    /// 微信公众号对接接口
    /// </summary>
    public class WechatController : Controller
    {
        private IWechatAdminApp _wechatAdminApp;
        private IWechatRequestLogApp _wechatRequestLogApp;
        public WechatController(IWechatAdminApp wechatAdminApp,
            IWechatRequestLogApp wechatRequestLogApp)
        {
            _wechatAdminApp = wechatAdminApp;
            _wechatRequestLogApp = wechatRequestLogApp;
        }

        /// <summary>
        /// 微信对接 post 接口入口
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Api(string id, WechatApiReqModel reqModel)
        {
            var wechatAdmin = await _wechatAdminApp.GetWechatAdminByAccountName(id);
            if (wechatAdmin != null)
            {
                if (checkSignature(wechatAdmin.Token, reqModel))
                {
                    //处理消息
                    if (reqModel.echostr.IsEmpty())
                    {
                        WXBizMsgCrypt crypt = new WXBizMsgCrypt(wechatAdmin.Token, wechatAdmin.EncodingAESKey, wechatAdmin.AppId);
                        string requestData = string.Empty;
                        string refRequestData = string.Empty;
                        using (var streamRead = new System.IO.StreamReader(Request.InputStream))
                        {
                            requestData = await new System.IO.StreamReader(Request.InputStream).ReadToEndAsync();
                        }
                        //解密
                        int cryptResult = crypt.DecryptMsg(reqModel.msg_signature, reqModel.timestamp, reqModel.nonce, requestData, ref refRequestData);
                        if (cryptResult != 0)
                        {
                            return Content("success");
                        }                       
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml(refRequestData);
                        Dictionary<string, string> resultDic = new Dictionary<string, string>();
                        foreach (XmlNode item in xmldoc.SelectSingleNode("xml").ChildNodes)
                        {
                            resultDic.Add(item.Name, item.InnerText);
                        }

                        await _wechatRequestLogApp.Insert(new Domain.Entity.T_Wechat_RequestLog()
                        {
                            WechatAdminId = wechatAdmin.ID,
                            MsgId = resultDic["MsgId"],
                            FromUserName = resultDic["FromUserName"],
                            FromCreateTime = resultDic["CreateTime"],
                            MsgType = resultDic["MsgType"],
                            Content = resultDic["Content"]
                        });

                        string result = string.Empty, refResult = string.Empty;
                        HandlerWebChatMessage.StartHanlder(resultDic, out result);
                        crypt.EncryptMsg(result, reqModel.timestamp, reqModel.nonce, ref refResult);
                        return Content(refResult);
                    }
                    else
                    {
                        //echostr 不为空则是微信接入请求返回echostr
                        return Content(reqModel.echostr);
                    }
                }
            }
            return Content("error-401");
        }

        /// <summary>
        /// 验证是否来自微信
        /// </summary>
        /// <param name="token"></param>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        private bool checkSignature(string token, WechatApiReqModel reqModel)
        {
            try
            {
                var signArr = new string[] { token, reqModel.timestamp, reqModel.nonce };
                Array.Sort(signArr);
                string signature = Sha1Helper.GetSHA1(string.Join("", signArr));
                return signature.Equals(reqModel.signature, StringComparison.CurrentCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}