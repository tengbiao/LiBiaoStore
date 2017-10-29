using LiBiaoStore.Domain.Entity;
using MySql.Data.Entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace LiBiaoStore.Data.CurrentContext
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class LibiaoDbContext : DbContext
    {
        public LibiaoDbContext() : this("LibiaoStoreContext")
        {

        }

        public LibiaoDbContext(string sqlConntionStr) : base(sqlConntionStr)
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #region  IDbSet<Entity>

        public IDbSet<T_Wechat_Admin> T_Wechat_Admin { set; get; }
        public IDbSet<T_Wechat_RequestLog> T_Wechat_RequestLog { set; get; }
        //public IDbSet<Sys_FilterIP> Sys_FilterIP { set; get; }
        //public IDbSet<Sys_Items> Sys_Items { set; get; }
        //public IDbSet<Sys_ItemsDetail> Sys_ItemsDetail { set; get; }
        //public IDbSet<Sys_Log> Sys_Log { set; get; }
        //public IDbSet<Sys_Module> Sys_Module { set; get; }
        //public IDbSet<Sys_ModuleButton> Sys_ModuleButton { set; get; }
        //public IDbSet<Sys_Organize> Sys_Organize { set; get; }
        //public IDbSet<Sys_Role> Sys_Role { set; get; }
        //public IDbSet<Sys_RoleAuthorize> Sys_RoleAuthorize { set; get; }
        //public IDbSet<Sys_User> Sys_User { set; get; }
        //public IDbSet<Sys_UserLogOn> Sys_UserLogOn { set; get; }

        #endregion

    }
}
