using LiBiaoStore.Application.IApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiBiaoStore.Domain.Entity;
using LiBiaoStore.Repository.IRepository;
using LiBiaoStore.Code;

namespace LiBiaoStore.Application.Application
{
    public class WechatRequestLogApp : IWechatRequestLogApp
    {
        IRepository<T_Wechat_RequestLog> _wechatRequestLogRepository;
        public WechatRequestLogApp(IRepository<T_Wechat_RequestLog> wechatRequestLogRepository)
        {
            _wechatRequestLogRepository = wechatRequestLogRepository;
        }

        public async Task<T_Wechat_RequestLog> Insert(T_Wechat_RequestLog entity)
        {
            entity.ID = Common.GuId();
            entity.CreateUser = "auto";
            entity.CreateTime = DateTime.Now;
            return await _wechatRequestLogRepository.InsertAsync(entity);
        }
    }
}
