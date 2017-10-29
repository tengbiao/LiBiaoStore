using LiBiaoStore.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiBiaoStore.Application.IApplication
{
    public interface IWechatAdminApp
    {
        Task<T_Wechat_Admin> GetWechatAdminByAccountName(string accountName);
    }
}
