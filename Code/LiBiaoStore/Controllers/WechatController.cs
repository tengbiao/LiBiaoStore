using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiBiaoStore.Web.Controllers
{
    /// <summary>
    /// 微信公众号对接接口
    /// </summary>
    public class WechatController : Controller
    {
        // GET: Wechat
        public ActionResult Index()
        {
            return View();
        }
    }
}