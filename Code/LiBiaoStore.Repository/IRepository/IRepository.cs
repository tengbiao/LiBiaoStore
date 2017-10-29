using LiBiaoStore.Data.CurrentContext;
using LiBiaoStore.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace LiBiaoStore.Repository.IRepository
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepository<TEntity> : IDbContextRepository<LibiaoDbContext,TEntity> where TEntity : class,new()
    {

    }
}
