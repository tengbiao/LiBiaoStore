using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace LiBiaoStore.Data.CurrentContext
{
    public class LibiaoDbContext : DbContext
    {
        public LibiaoDbContext() 
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
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

        //public IDbSet<Sys_Area> Sys_Area { set; get; }
        //public IDbSet<Sys_DbBackup> Sys_DbBackup { set; get; }
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
