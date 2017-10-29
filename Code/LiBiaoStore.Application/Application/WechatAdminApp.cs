using LiBiaoStore.Application.IApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiBiaoStore.Domain.Entity;
using LiBiaoStore.Repository.IRepository;

namespace LiBiaoStore.Application.Application
{
    public class WechatAdminApp : IWechatAdminApp
    {
        IRepository<T_Wechat_Admin> _wechatAdminRepository;
        public WechatAdminApp(IRepository<T_Wechat_Admin> wechatAdminRepository)
        {
            _wechatAdminRepository = wechatAdminRepository;
        }

        /// <summary>
        /// 根据微信对应的accountName获得对应的信息
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task<T_Wechat_Admin> GetWechatAdminByAccountName(string accountName)
        {
            return await _wechatAdminRepository.FindEntityAsync(q => q.AccountName == accountName);
        }
    }
}
