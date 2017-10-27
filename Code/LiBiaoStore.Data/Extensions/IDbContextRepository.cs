using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LiBiaoStore.Data.Extensions
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbContextRepository<TDbContext, TEntity> where TEntity : class, new() where TDbContext : DbContext, new()
    {
        TDbContext GetDbContext();
        DbSet<TEntity> GetTable();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        int Insert(List<TEntity> entitys);
        Task<int> InsertAsync(List<TEntity> entitys);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        int Delete(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        int Delete(Expression<Func<TEntity, bool>> predicate);
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity FindKey(object keyValue);
        Task<TEntity> FindKeyAsync(object keyValue);
        TEntity FindEntity(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindEntityAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> IQueryable();
        IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> FindList(string strSql);
        Task<List<TEntity>> FindListAsync(string strSql);
        List<TEntity> FindList(string strSql, DbParameter[] dbParameter);
        Task<List<TEntity>> FindListAsync(string strSql, DbParameter[] dbParameter);
        List<TEntity> FindList(Pagination pagination);
        Task<List<TEntity>> FindListAsync(Pagination pagination);
        List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination);
        Task<List<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> predicate, Pagination pagination);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

    }
}
