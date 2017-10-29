using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;


namespace LiBiaoStore.Code
{
    public class MPHelper
    {
        /// <summary>
        /// 错误码集合
        /// </summary>
        private static System.Collections.Hashtable err = new System.Collections.Hashtable();
        private static ICache _cache = CacheFactory.Cache();
        private static string saveDir = "";//ConfigurationManager.AppSettings["SaveFilter"].ToString();
        #region 自定义菜单
        /// <summary>
        /// 初始化接口信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="token"></param>
        /// <returns></returns>

        public static Result Init(string appid, string secret, out string token)
        {
            Result rlt = new Result();
            token = "";
            if (err.Count == 0)
            {
                err.Add("", "");
                err.Add(" -1", "系统繁忙");
                err.Add("0", "请求成功");
                err.Add("40001", "验证失败");
                err.Add("40002", "不合法的凭证类型");
                err.Add("40003", "不合法的OpenID");
                err.Add("40004", "不合法的媒体文件类型");
                err.Add("40005", "不合法的文件类型");
                err.Add("40006", "不合法的文件大小");
                err.Add("40007", "不合法的媒体文件id");
                err.Add("40008", "不合法的消息类型");
                err.Add("40009", "不合法的图片文件大小");
                err.Add("40010", "不合法的语音文件大小");
                err.Add("40011", "不合法的视频文件大小");
                err.Add("40012", "不合法的缩略图文件大小");
                err.Add("40013", "不合法的APPID");
                err.Add("40014", "不合法的access_token");
                err.Add("40015", "不合法的菜单类型");
                err.Add("40016", "不合法的按钮个数");
                err.Add("40017", "不合法的按钮个数");
                err.Add("40018", "不合法的按钮名字长度");
                err.Add("40019", "不合法的按钮KEY长度");
                err.Add("40020", "不合法的按钮URL长度");
                err.Add("40021", "不合法的菜单版本号");
                err.Add("40022", "不合法的子菜单级数");
                err.Add("40023", "不合法的子菜单按钮个数");
                err.Add("40024", "不合法的子菜单按钮类型");
                err.Add("40025", "不合法的子菜单按钮名字长度");
                err.Add("40026", "不合法的子菜单按钮KEY长度");
                err.Add("40027", "不合法的子菜单按钮URL长度");
                err.Add("40028", "不合法的自定义菜单使用用户");
                err.Add("41001", "缺少access_token参数");
                err.Add("41002", "缺少appid参数");
                err.Add("41003", "缺少refresh_token参数");
                err.Add("41004", "缺少secret参数");
                err.Add("41005", "缺少多媒体文件数据");
                err.Add("41006", "缺少media_id参数");
                err.Add("41007", "缺少子菜单数据");
                err.Add("42001", "access_token超时");
                err.Add("43001", "需要GET请求");
                err.Add("43002", "需要POST请求");
                err.Add("43003", "需要HTTPS请求");
                err.Add("44001", "多媒体文件为空");
                err.Add("44002", "POST的数据包为空");
                err.Add("44003", "图文消息内容为空");
                err.Add("45001", "多媒体文件大小超过限制");
                err.Add("45002", "消息内容超过限制");
                err.Add("45003", "标题字段超过限制");
                err.Add("45004", "描述字段超过限制");
                err.Add("45005", "链接字段超过限制");
                err.Add("45006", "图片链接字段超过限制");
                err.Add("45007", "语音播放时间超过限制");
                err.Add("45008", "图文消息超过限制");
                err.Add("45009", "接口调用超过限制");
                err.Add("45010", "创建菜单个数超过限制");
                err.Add("45015", "回复时间超过限制");
                err.Add("46001", "不存在媒体数据");
                err.Add("46002", "不存在的菜单版本");
                err.Add("46003", "不存在的菜单数据");
                err.Add("47001", "解析JSON/XML内容错误");
            }
            try
            {
                string tempToken = _cache.GetCache<string>(appid);
                if (tempToken.IsEmpty())
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
                    string result = HttpMethods.HttpGet(url);
                    try
                    {
                        GetToken gtk = result.ToObject<GetToken>();
                        token = gtk.access_token;
                        //var expires_in = gtk.expires_in;
                        if (string.IsNullOrEmpty(token))
                        {
                            rlt.Add(err["40001"].ToString());
                        }
                        else
                        {
                            //MemoryCache.Set(appid, token, 7000);
                        }
                    }
                    catch
                    {
                        Msg msg = result.ToObject<Msg>();
                        if (msg.errcode != "0")
                        {
                            rlt.Add(err[msg.errcode].ToString());
                        }
                    }
                }
                else
                {
                    token = tempToken.ToString();
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }

        /// <summary>
        /// 获得微信Token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static String GetToken(string appid, string secret)
        {
            string token = "";
            Init(appid, secret, out token);
            return token;
        }


        /// <summary>
        /// 同步自定义菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Result SyncMenu(string appid, string secret, string json)
        {
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", token);
                string result = HttpMethods.HttpPost(url, json);

                Msg msg = result.ToObject<Msg>();
                if (msg.errcode != "0")
                {
                    rlt.Add(err[msg.errcode].ToString());
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }
        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Result GetMenu(string appid, string secret, out string json)
        {
            json = "";
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", token);
                json = HttpMethods.HttpGet(url);
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }
        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static Result DelMenu(string appid, string secret)
        {
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", token);
                string result = HttpMethods.HttpPost(url);

                Msg msg = JsonHelper.Deserialize<Msg>(result);
                if (msg.errcode != "0")
                {
                    rlt.Add(err[msg.errcode].ToString());
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }
        #endregion 自定义菜单

        #region 用户相关
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="openid"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Result GetUserInfo(string appid, string secret, string openid, out string json)
        {
            json = "";
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                if (rlt.IsValid)
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", token, openid);
                    json = PostAndGet.GetResponseString(url);
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }

        /// <summary>
        /// 获得关注的用户列表
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="nextopenid"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Result GetUserList(string appid, string secret, string nextopenid, out string json)
        {
            json = "";
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                if (rlt.IsValid)
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}", token, nextopenid);
                    json = PostAndGet.GetResponseString(url);
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }

        #endregion

        #region 发送消息相关
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="appid">微信AppId</param>
        /// <param name="secret">微信Secret</param>
        /// <param name="openid">接受用户的OpenId</param>
        /// <param name="text">要发送的文本</param>
        /// <returns></returns>
        public static Result SendText(string appid, string secret, string openid, string text)
        {
            string json = "{\"touser\":\"" + openid + "\",\"msgtype\":\"text\",\"text\":{\"content\":\"" + text + "\"}}";
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", token);
                string result = PostAndGet.PostWebRequest(url, json, "utf-8");

                Msg msg = JsonHelper.Deserialize<Msg>(result);
                if (msg.errcode != "0")
                {
                    rlt.Add(err[msg.errcode].ToString());
                }
            }
            catch (Exception ex)
            {
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="appid">微信AppId</param>
        /// <param name="secret">微信Secret</param>
        /// <param name="postjson">要发送的Json数据包</param>
        /// <returns></returns>
        public static Result SendTemplate(string appid, string secret, string postjson)
        {
            #region postjson格式
            //      {
            //    "touser":"OPENID",
            //    "template_id":"ngqIpbwh8bUfcSsECmogfXcV14J0tQlEpBO27izEYtY",
            //    "url":"http://weixin.qq.com/download",
            //    "topcolor":"#FF0000",
            //    "data":{
            //            "first": {
            //                "value":"恭喜你购买成功！",
            //                "color":"#173177"
            //            },
            //            "keynote1":{
            //                "value":"巧克力",
            //                "color":"#173177"
            //            },
            //            "keynote2": {
            //                "value":"39.8元",
            //                "color":"#173177"
            //            },
            //            "keynote3": {
            //                "value":"2014年9月16日",
            //                "color":"#173177"
            //            },
            //            "remark":{
            //                "value":"欢迎再次购买！",
            //                "color":"#173177"
            //            }
            //    }
            //}
            #endregion
            Result rlt = new Result();
            try
            {
                string token = "";
                rlt.Join(Init(appid, secret, out token));
                string url = string.Format(" https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", token);
                string result = PostAndGet.PostWebRequest(url, postjson, "utf-8");

                Msg msg = JsonHelper.Deserialize<Msg>(result);
                if (msg.errcode != "0")
                {
                    rlt.Add(err[msg.errcode].ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Log.Error(ex.Message, ex);
                rlt.Add("错误：" + ex.Message);
            }
            return rlt;
        }

        #endregion

        #region Js接口相关,网页授权
        /// <summary>
        /// 获得js接口授权需要的JSApi Ticket,7200s时效
        /// </summary>
        /// <param name="appid">公众生成的AppID</param>
        /// <param name="secret">公众号生成的AppSecret</param>
        /// <returns></returns>
        public static String GetJsapi_ticket(string appid, string secret)
        {
            object jsapi = "";
            try
            {
                object tempToken = MemoryCache.Get("CacheJsapi" + appid);
                if (tempToken == null || tempToken.ToString() == "")
                {
                    string token = GetToken(appid, secret);
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", token);
                    string result = PostAndGet.GetResponseString(url);
                    try
                    {
                        jsapi = JsonHelper.GetField(result, "ticket");
                        if (jsapi != null && jsapi.ToString() != "")
                            MemoryCache.Set("CacheJsapi" + appid, jsapi, 7000);
                    }
                    catch
                    {
                        Msg msg = JsonHelper.Deserialize<Msg>(result);
                        if (msg.errcode != "0")
                        {
                            jsapi = err[msg.errcode];
                        }
                    }
                }
                else
                {
                    jsapi = tempToken;
                }
            }
            catch
            {
                jsapi = "";
            }
            return jsapi.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="callbackurl"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ResposeToPageAuthrized(string appid, string callbackurl, string state = "")
        {
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect";
            url = string.Format(url, appid, callbackurl, state);
            return url;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetOpenIdByCode(string appid, string secret, string code)
        {
            bool wechatopen = true;
            Boolean.TryParse(ConfigHelper.GetAppSettings("WechatOpened"), out wechatopen);
            string url = string.Empty;
            string openid = string.Empty;
            url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + code + "&grant_type=authorization_code";
            openid = PostAndGet.GetResponseString(url);
            openid = JsonHelper.GetFieldStr(openid, "openid");
            return openid;
        }

        #endregion

        #region 下载相关
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="secret">secret</param>
        /// <param name="mediaId">媒体ID,即serverId</param>        
        /// <param name="directory">写出保存的文件路径</param>
        /// <param name="fileName"></param>
        public static string DownloadFile(string appid, string secret, string mediaId, string directory, string fileName = "")
        {
            string token = "";
            Init(appid, secret, out token);
            string urlFormat = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";
            string url = string.Format(urlFormat, token, mediaId);
            System.Net.HttpWebResponse response = null;
            System.Net.HttpWebRequest request = null;
            try
            {
                request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.ServicePoint.Expect100Continue = false;
                request.Method = "GET";
                request.KeepAlive = true;
                request.UserAgent = "Image";
                request.Timeout = 100000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    string disp = response.Headers.Get("Content-disposition");
                    string ext = disp.Substring(disp.LastIndexOf("."));
                    ext = ext.Substring(0, ext.Length - 1);
                    //创建根目录
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    if (string.IsNullOrEmpty(fileName))
                        fileName = System.Guid.NewGuid().ToString() + ext;
                    string saveFileName = directory + fileName;
                    //string makedir = saveFileName.Insert(saveFileName.LastIndexOf("."), "_thumb");//缩略图全路径

                    //保存原图
                    SaveBinaryFile(response, saveFileName);
                    //保存缩略图
                    //Util.MakeThumbnail(saveFileName, makedir, 100, 100, "W");

                    //返回缩略图路径
                    //saveFileName = makedir;
                    return fileName;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request = null;
                }
            }
            return null;
            // return saveFileName;
        }

        /// <summary>
        ///  将二进制文件保存到磁盘
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool SaveBinaryFile(System.Net.WebResponse response, string fileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                Stream outStream = System.IO.File.Create(fileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);
                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }

        #endregion

        #region 微信支付相关

        /// <summary>
        /// 微信发送红包接口
        /// </summary>
        /// <param name="billno">系统业务10位流水号，一天内不能有重复项</param>
        /// <param name="openid">发放给哪个用户</param>
        /// <param name="totalamout">发送的金额（单位元）</param>
        /// <param name="result">返回结果</param>
        /// <returns></returns>
        public static Result SendRedPack(string billno, string openid, double totalamout, string appId, string mchid, string apiKey, string cert, out string result)
        {
            Result rlt = new Result();
            string mchbillno = mchid + DateTime.Now.ToString("yyyyMMdd") + billno;
            string xmldata = RedPackXml(mchbillno, mchid, apiKey, appId, openid, totalamout);
            result = "";
            string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
            try
            {
                Encoding encode = Encoding.Default;
                encode = System.Text.Encoding.GetEncoding("utf-8");
                byte[] byteArray = encode.GetBytes(xmldata); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                //添加证书
                //@"E:\C#Worker\wolvesbrand\apiclient_cert.p12";
                string password = mchid;
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //X509Certificate cer = new X509Certificate(cert, password);
                X509Certificate2 cer = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert, password, X509KeyStorageFlags.MachineKeySet);

                CookieContainer cookieContainer = new CookieContainer();
                webReq.CookieContainer = cookieContainer;
                webReq.AllowAutoRedirect = true;
                webReq.Method = "POST";
                webReq.ContentType = "text/xml";
                webReq.ContentLength = byteArray.Length;
                webReq.ClientCertificates.Add(cer);

                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), encode);
                string ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
                result = ret;
            }
            catch (Exception ex)
            {
                Log.Log.Error(ex.Message, ex);
                rlt.Add(ex.Message);
            }
            return rlt;
        }

        /// <summary>
        ///  微信发送红包接口
        /// </summary>
        /// <param name="info">string billno, string openid, double totalamout, string appId, string mchid, string apiKey, string cert</param>
        /// <param name="result">输入发送结果</param>
        /// <returns></returns>
        public static Result SendRedPack(RedPackInfo info, out string result)
        {
            Result rlt = new Result();
            string xmldata = RedPackXml(info);
            result = "";
            string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
            try
            {
                Encoding encode = Encoding.Default;
                encode = System.Text.Encoding.GetEncoding("utf-8");
                byte[] byteArray = encode.GetBytes(xmldata); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                //添加证书
                //@"E:\C#Worker\wolvesbrand\apiclient_cert.p12";
                string password = info.mchid;
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //X509Certificate cer = new X509Certificate(cert, password);
                X509Certificate2 cer = new System.Security.Cryptography.X509Certificates.X509Certificate2(info.cert, password, X509KeyStorageFlags.MachineKeySet);

                CookieContainer cookieContainer = new CookieContainer();
                webReq.CookieContainer = cookieContainer;
                webReq.AllowAutoRedirect = true;
                webReq.Method = "POST";
                webReq.ContentType = "text/xml";
                webReq.ContentLength = byteArray.Length;
                webReq.ClientCertificates.Add(cer);

                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), encode);
                string ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
                result = ret;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                Log.Log.Error(ex.Message, ex);
                rlt.Add(ex.Message);
            }
            return rlt;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>
        /// 返回发送微信红包需要的xml数据包
        /// </summary>
        /// <param name="mchbillno">系统订单号10位</param>
        /// <param name="mchid">商户号</param>
        /// <param name="apikey">商户密钥</param>
        /// <param name="wxappid">微信公众平台appid</param>
        /// <param name="reopenid">接受人的OpenId</param>
        /// <param name="totalamount">发送的金额数（单位：元）</param>
        /// <returns></returns>
        private static string RedPackXml(string mchbillno, string mchid, string apikey, string wxappid, string reopenid, double totalamount)
        {
            //获得所需参数
            string actname = "微信摇一摇";
            string nickname = "上海狼群";
            string sendname = "上海狼群";
            string wishing = "欢迎您参加活动，祝你天天开心";
            string clientip = "127.0.0.1";
            string remark = "越摇越开心";
            string noncestr = Rand.Str(32);
            Int64 totalamounttemp = (Int64)(totalamount * 100);
            //生成Sign签名
            //对参数按照key=value的格式，并按照参数名ASCII字典序排序如下
            String[] signtemp =
            {
                "mch_billno="+mchbillno,
                "mch_id="+mchid,
                "wxappid="+wxappid,
                "nick_name="+nickname,
                "send_name="+sendname,
                "re_openid="+reopenid,
                "total_amount="+totalamounttemp,
                "min_value="+totalamounttemp,
                "max_value="+totalamounttemp,
                "total_num=1",
                "wishing="+wishing,
                "client_ip="+clientip,
                "act_name="+actname,
                "remark="+remark,
                "nonce_str="+noncestr
            };
            Array.Sort(signtemp);
            string signTempStr = String.Join("&", signtemp);
            //拼接API密钥：
            string stringSignTemp = signTempStr + "&key=" + apikey;
            string sign = EncryptorHelper.Md5Encryptor32(stringSignTemp).ToUpper();

            //生成xml包
            StringBuilder sb = new StringBuilder(0);
            sb.Append("<xml>");
            sb.AppendFormat("<sign>{0}</sign>", sign);//签名
            sb.AppendFormat("<mch_billno>{0}</mch_billno>", mchbillno);//商户订单号（每个订单号必须唯一）组成： mch_id+yyyymmdd+10位一天内不能重复的数字。接口根据商户订单号支持重入， 如出现超时可再调用。
            sb.AppendFormat("<mch_id>{0}</mch_id>", mchid);//微信支付分配的商户号
            sb.AppendFormat("<wxappid>{0}</wxappid>", wxappid);//商户appid
            sb.AppendFormat("<nick_name>{0}</nick_name>", nickname);//提供方名称
            sb.AppendFormat("<send_name>{0}</send_name>", sendname);//红包发送者名称
            sb.AppendFormat("<re_openid>{0}</re_openid>", reopenid);//接受收红包的用户
            sb.AppendFormat("<total_amount>{0}</total_amount>", totalamounttemp);//付款金额，单位分
            sb.AppendFormat("<min_value>{0}</min_value>", totalamounttemp);//最小红包金额，单位分
            sb.AppendFormat("<max_value>{0}</max_value>", totalamounttemp);//最大红包金额，单位分（ 最小金额等于最大金额： min_value=max_value =total_amount）
            sb.AppendFormat("<total_num>1</total_num>");//红包发放总人数total_num=1
            sb.AppendFormat("<wishing>{0}</wishing>", wishing);//红包祝福语
            sb.AppendFormat("<client_ip>{0}</client_ip>", clientip);//调用接口的机器Ip地址
            sb.AppendFormat("<act_name>{0}</act_name>", actname);//活动名称
            // sb.AppendFormat("<act_id></act_id>");          
            sb.AppendFormat("<remark>{0}</remark>", remark);//备注信息
            //sb.AppendFormat("<logo_imgurl></logo_imgurl>");//商户logo的url
            //sb.AppendFormat("<share_content></share_content>");//分享文案
            //sb.AppendFormat("<share_url></share_url>");//分享链接
            //sb.AppendFormat("<share_imgurl></share_imgurl>");//分享的图片url
            sb.AppendFormat("<nonce_str>{0}</nonce_str>", noncestr);//随机字符串，不长于32位
            sb.AppendFormat("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 返回发送微信红包需要的xml数据包
        /// </summary>
        /// <param name="mchbillno">系统订单号10位</param>
        /// <param name="mchid">商户号</param>
        /// <param name="apikey">商户密钥</param>
        /// <param name="wxappid">微信公众平台appid</param>
        /// <param name="reopenid">接受人的OpenId</param>
        /// <param name="totalamount">发送的金额数（单位：元）</param>
        /// <returns></returns>
        private static string RedPackXml(RedPackInfo info)
        {
            //获得所需参数
            //string actname = "微信摇一摇";
            //string nickname = "上海狼群";
            //string sendname = "上海狼群";
            //string wishing = "欢迎您参加活动，祝你天天开心";
            //string clientip = "127.0.0.1";
            //string remark = "越摇越开心";
            //string noncestr = Rand.Str(32);
            Int64 totalamounttemp = (Int64)(info.totalamout * 100);
            //生成Sign签名
            //对参数按照key=value的格式，并按照参数名ASCII字典序排序如下
            String[] signtemp =
            {
                "mch_billno="+info.mchbillno,
                "mch_id="+info.mchid,
                "wxappid="+info.appId,
                "nick_name="+info.nickname,
                "send_name="+info.sendname,
                "re_openid="+info.openid,
                "total_amount="+totalamounttemp,
                "min_value="+totalamounttemp,
                "max_value="+totalamounttemp,
                "total_num=1",
                "wishing="+info.wishing,
                "client_ip="+info.clientip,
                "act_name="+info.actname,
                "remark="+info.remark,
                "nonce_str="+info.noncestr,
                "scene_id="+info.scene_id
            };
            Array.Sort(signtemp);
            string signTempStr = String.Join("&", signtemp);
            //拼接API密钥：
            string stringSignTemp = signTempStr + "&key=" + info.apiKey;
            string sign = EncryptorHelper.Md5Encryptor32(stringSignTemp).ToUpper();

            //生成xml包
            StringBuilder sb = new StringBuilder(0);
            sb.Append("<xml>");
            sb.AppendFormat("<sign>{0}</sign>", sign);//签名
            sb.AppendFormat("<mch_billno>{0}</mch_billno>", info.mchbillno);//商户订单号（每个订单号必须唯一）组成： mch_id+yyyymmdd+10位一天内不能重复的数字。接口根据商户订单号支持重入， 如出现超时可再调用。
            sb.AppendFormat("<mch_id>{0}</mch_id>", info.mchid);//微信支付分配的商户号
            sb.AppendFormat("<wxappid>{0}</wxappid>", info.appId);//商户appid
            sb.AppendFormat("<nick_name>{0}</nick_name>", info.nickname);//提供方名称
            sb.AppendFormat("<send_name>{0}</send_name>", info.sendname);//红包发送者名称
            sb.AppendFormat("<re_openid>{0}</re_openid>", info.openid);//接受收红包的用户
            sb.AppendFormat("<total_amount>{0}</total_amount>", totalamounttemp);//付款金额，单位分
            sb.AppendFormat("<min_value>{0}</min_value>", totalamounttemp);//最小红包金额，单位分
            sb.AppendFormat("<max_value>{0}</max_value>", totalamounttemp);//最大红包金额，单位分（ 最小金额等于最大金额： min_value=max_value =total_amount）
            sb.AppendFormat("<total_num>1</total_num>");//红包发放总人数total_num=1
            sb.AppendFormat("<wishing>{0}</wishing>", info.wishing);//红包祝福语
            sb.AppendFormat("<client_ip>{0}</client_ip>", info.clientip);//调用接口的机器Ip地址
            sb.AppendFormat("<act_name>{0}</act_name>", info.actname);//活动名称
            // sb.AppendFormat("<act_id></act_id>");
            sb.AppendFormat("<remark>{0}</remark>", info.remark);//备注信息
            sb.AppendFormat("<scene_id>{0}</scene_id>", info.scene_id);
            //sb.AppendFormat("<logo_imgurl></logo_imgurl>");//商户logo的url
            //sb.AppendFormat("<share_content></share_content>");//分享文案
            //sb.AppendFormat("<share_url></share_url>");//分享链接
            //sb.AppendFormat("<share_imgurl></share_imgurl>");//分享的图片url
            sb.AppendFormat("<nonce_str>{0}</nonce_str>", info.noncestr);//随机字符串，不长于32位
            sb.AppendFormat("</xml>");
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 清除Access——token 和JsTicket 缓存信息
        /// </summary>
        /// <param name="appid"></param>
        public static void RemoveWechatCache(string appid)
        {
            MemoryCache.Remove("CacheJsapi" + appid);
            MemoryCache.Remove(appid);
        }

    }

    public class GetToken
    {
        private string _access_token;
        private string _expires_in;

        public string access_token { get { return _access_token; } set { _access_token = value; } }

        public string expires_in { get { return _expires_in; } set { _expires_in = value; } }
    }

    public class Msg
    {
        private string _errcode;
        private string _errmsg;

        public string errcode { get { return _errcode; } set { _errcode = value; } }

        public string errmsg { get { return _errmsg; } set { _errmsg = value; } }
    }

    public class UserModel
    {
        public string openid { set; get; }
        public string nickname { set; get; }
        public string sex { set; get; }
        public string province { set; get; }
        public string city { set; get; }
        public string country { set; get; }
        public string headimgurl { set; get; }
        public string[] privilege { set; get; }
        public string unionid { set; get; }
    }
}

