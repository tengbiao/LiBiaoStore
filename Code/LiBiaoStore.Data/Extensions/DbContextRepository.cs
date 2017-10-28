using LiBiaoStore.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiBiaoStore.Data.Extensions
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    public class DbContextRepository<TDbContext, TEntity> : IDbContextRepository<TDbContext, TEntity>
        where TDbContext : DbContext, new()
        where TEntity : class, new()
    {
        protected TDbContext Context
        {
            get
            {
                //获取BaseDbContext的完全限定名，其实这个名字没什么特别的意义，仅仅是一个名字而已，也可以取别的名字的  
                string threadName = typeof(TDbContext).FullName;
                //获取key为threadName的这个线程缓存（CallContext就是线程缓存容器类）  
                object dbObj = CallContext.GetData(threadName);
                //如果key为threadName的线程缓存不存在  
                if (dbObj == null)
                {
                    //创建BaseDbContext类的对象实例  
                    dbObj = new TDbContext();
                    //将这个BaseDbContext类的对象实例保存到线程缓存当中(以键值对的形式进行保存的，我这就将key设为当前线程的完全限定名了)  
                    CallContext.SetData(threadName, dbObj);
                    return dbObj as TDbContext;
                }
                return dbObj as TDbContext;
            }
        }

        public TDbContext GetDbContext()
        {
            return Context;
        }

        public DbSet<TEntity> Table
        {
            get
            {
                return Context.Set<TEntity>();
            }
        }
        public DbSet<TEntity> GetTable()
        {
            return Table;
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        /// <param name="islazy">是否延迟提交 默认立即提交， 当传true时LazySaveChange属性必须为true</param>
        /// <returns></returns>
        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        /// <param name="islazy">是否延迟提交 默认立即提交， 当传true时LazySaveChange属性必须为true</param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public TEntity Insert(TEntity entity)
        {
            Context.Entry<TEntity>(entity).State = EntityState.Added;
            SaveChanges();
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            Context.Entry<TEntity>(entity).State = EntityState.Added;
            await SaveChangesAsync();
            return entity;
        }

        public int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                Context.Entry<TEntity>(entity).State = EntityState.Added;
            }
            return SaveChanges();
        }

        public async Task<int> InsertAsync(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                Context.Entry<TEntity>(entity).State = EntityState.Added;
            }
            return await SaveChangesAsync();
        }

        public TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                        Context.Entry(entity).Property(prop.Name).CurrentValue = null;
                    Context.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            int result = SaveChanges();
            return result > 0 ? entity : null;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            Context.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                        Context.Entry(entity).Property(prop.Name).CurrentValue = null;
                    Context.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            int result = await SaveChangesAsync();
            return result > 0 ? entity : null;
        }

        public void AddOrUpdate(params TEntity[] entitys)
        {
            Table.AddOrUpdate(entitys);
        }

        public int Delete(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry<TEntity>(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry<TEntity>(entity).State = EntityState.Deleted;
            int result = await SaveChangesAsync();
            return result;
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitys = Table.Where(predicate).ToList();
            entitys.ForEach(m => Context.Entry<TEntity>(m).State = EntityState.Deleted);
            return SaveChanges();
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entitys = Table.Where(predicate).ToList();
            entitys.ForEach(m => Context.Entry<TEntity>(m).State = EntityState.Deleted);
            return await SaveChangesAsync();
        }

        public TEntity FindKey(object keyValue)
        {
            return Table.Find(keyValue);
        }

        public async Task<TEntity> FindKeyAsync(object keyValue)
        {
            return await Table.FindAsync(keyValue);
        }

        public TEntity FindEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.FirstOrDefault(predicate);
        }

        public async Task<TEntity> FindEntityAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> IQueryable()
        {
            return Table.AsQueryable();
        }

        public IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate);
        }

        public List<TEntity> FindList(string strSql)
        {
            return Context.Database.SqlQuery<TEntity>(strSql).ToList<TEntity>();
        }

        public async Task<List<TEntity>> FindListAsync(string strSql)
        {
            return await Context.Database.SqlQuery<TEntity>(strSql).ToListAsync();
        }

        public List<TEntity> FindList(string strSql, DbParameter[] dbParameter)
        {
            return Context.Database.SqlQuery<TEntity>(strSql, dbParameter).ToList();
        }

        public async Task<List<TEntity>> FindListAsync(string strSql, DbParameter[] dbParameter)
        {
            return await Context.Database.SqlQuery<TEntity>(strSql, dbParameter).ToListAsync();
        }

        public List<TEntity> FindList(Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Context.Set<TEntity>().AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.ToList();
        }

        public async Task<List<TEntity>> FindListAsync(Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Context.Set<TEntity>().AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return await tempData.ToListAsync();
        }

        public List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Context.Set<TEntity>().Where(predicate);
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.ToList();
        }

        public async Task<List<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = Context.Set<TEntity>().Where(predicate);
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return await tempData.ToListAsync();
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }
        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await Context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        #region 备用方法 ，尽量少用

        //public int ExecuteNonQueryProc(string procName, params object[] parameters)
        //{
        //    try
        //    {
        //        var cmd = Context.Database.Connection.CreateCommand();
        //        Context.Database.Connection.Open();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = procName;
        //        cmd.Parameters.AddRange(parameters);
        //        cmd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
        //        int rows = cmd.ExecuteNonQuery();
        //        int result = (int)cmd.Parameters["ReturnValue"].Value;
        //        return rows;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        Context.Database.Connection.Close();
        //    }
        //}

        //public int ExecuteNonQuerySql(string sql, params object[] parameters)
        //{
        //    try
        //    {
        //        var cmd = Context.Database.Connection.CreateCommand();
        //        Context.Database.Connection.Open();
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandText = sql;
        //        if (parameters != null)
        //            cmd.Parameters.AddRange(parameters);
        //        int rows = cmd.ExecuteNonQuery();
        //        return rows;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        Context.Database.Connection.Close();
        //    }
        //}


        //public object ExecuteQuerySql(string sql, params object[] parameters)
        //{
        //    try
        //    {
        //        var cmd = Context.Database.Connection.CreateCommand();
        //        Context.Database.Connection.Open();
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandText = sql;
        //        cmd.Parameters.AddRange(parameters);
        //        return cmd.ExecuteScalar();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        Context.Database.Connection.Close();
        //    }
        //}

        //public DataSet ExecteDataSetProc(string procName, string tableName, params object[] parameters)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        conn = new SqlConnection(context.Database.Connection.ConnectionString);
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandText = procName;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = conn;
        //        if (parameters != null)
        //            cmd.Parameters.AddRange(parameters);
        //        SqlDataAdapter sqlDater = new SqlDataAdapter(cmd);
        //        sqlDater.Fill(ds, tableName);
        //        return ds;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        //public DataSet ExecteDataSetSql(string sql, string tableName, params object[] parameters)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        conn = new SqlConnection(context.Database.Connection.ConnectionString);
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandText = sql;
        //        cmd.Connection = conn;
        //        if (parameters != null)
        //            cmd.Parameters.AddRange(parameters);
        //        SqlDataAdapter sqlDater = new SqlDataAdapter(cmd);
        //        sqlDater.Fill(ds, tableName);
        //        return ds;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        //public DataSet ExecteDataSetSql(string sql, string tableName, int time = 0, params object[] parameters)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        conn = new SqlConnection(context.Database.Connection.ConnectionString);
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandText = sql;
        //        if (time > 0)
        //            cmd.CommandTimeout = time;
        //        cmd.Connection = conn;
        //        if (parameters != null)
        //            cmd.Parameters.AddRange(parameters);
        //        SqlDataAdapter sqlDater = new SqlDataAdapter(cmd);
        //        sqlDater.Fill(ds, tableName);
        //        return ds;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        #endregion 
    }
}
