using LiBiaoStore.Data.CurrentContext;
using LiBiaoStore.Data.Extensions;
using LiBiaoStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LiBiaoStore.Repository.Repository
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    public class Repository<TEntity> : DbContextRepository<LibiaoDbContext, TEntity>, IRepository<TEntity> where TEntity : class, new()
    {

    }
}
