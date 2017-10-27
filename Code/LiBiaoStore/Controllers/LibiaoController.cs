using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiBiaoStore.Web.Controllers
{
    /// <summary>
    /// 网站Controller
    /// </summary>
    public class LibiaoController : Controller
    {
        // GET: Libiao
        public ActionResult Index()
        {
            return View();
        }
    }
}