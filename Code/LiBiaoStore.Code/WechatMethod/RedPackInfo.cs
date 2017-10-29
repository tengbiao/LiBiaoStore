using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiBiaoStore.Code
{
    public class RedPackInfo
    {
        private string _mchbillno;
        public string mchbillno
        {
            get
            {
                if (string.IsNullOrEmpty(_mchbillno))
                    _mchbillno = mchid + DateTime.Now.ToString("yyyyMMdd") + billno;
                return _mchbillno;
            }
        }
        public string billno { set; get; }
        public string openid { set; get; }
        public double totalamout { set; get; }
        public string appId { set; get; }
        public string mchid { set; get; }
        public string apiKey { set; get; }
        public string cert { set; get; }
        public string actname { set; get; }
        public string nickname { set; get; }
        public string sendname { set; get; }
        public string wishing { set; get; }
        public string clientip { get { return "127.0.0.1"; } }
        public string remark { set; get; }
        private string _scene_id;

        public string scene_id
        {
            set
            {
                _scene_id = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_scene_id))
                    _scene_id = SceneType.PRODUCT_2;
                return _scene_id;
            }
        }

        public int userid { set; get; }

        private string _noncestr;
        public string noncestr
        {
            get
            {
                if (string.IsNullOrEmpty(_noncestr))
                    _noncestr = Rand.Str(32);
                return _noncestr;
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class SceneType
    {
        /// <summary>
        /// 场景一
        /// </summary>
        public const string PRODUCT_1 = "PRODUCT_1";
        /// <summary>
        /// 场景二
        /// </summary>
        public const string PRODUCT_2 = "PRODUCT_2";
    }
}
