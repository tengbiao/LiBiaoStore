using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Web.Mvc;
using System.Linq;
using LiBiaoStore.Repository.Repository;
using LiBiaoStore.Repository.IRepository;
using System.Web.Compilation;
using System.Reflection;

namespace LiBiaoStore.Web.App_Start
{
    public class AutoFacConfig
    {
        /// <summary>  
        /// 初始化AutoFac容器的相关数据  
        /// </summary>  
        public static void Register()
        {
            //初始化AutoFac的相关功能  
            /* 
             1.0 告诉AutoFac初始化数据仓储层BH.Repository.dll中所有类的对象实例。这些对象实例以其接口的形式保存在AutoFac容器中 
             2.0 告诉AutoFac初始化业务逻辑层BH.Application.dll中所有类的对象实例。这些对象实例以其接口的形式保存在AutoFac容器中 
             3.0 将MVC默认的控制器工厂替换成AutoFac的工厂 
             */

            //第一步： 构造一个AutoFac的builder容器  
            ContainerBuilder builder = new ContainerBuilder();
            //注册仓储基类
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();

            //注册控制器
            builder.RegisterControllers(assemblys.ToArray());
            
            //注册所有的 AppService 和 Repository
            foreach (var item in assemblys)
            {
                string name = item.GetName().Name;
                if (new string[] { "LiBiaoStore.Application", "LiBiaoStore.Repository" }.Count(a => a == name) > 0)
                {
                    Type[] stypes = item.GetTypes();
                    builder.RegisterTypes(stypes).Where(i => i.Name.EndsWith("App")
                    || i.Name.EndsWith("Repository")).AsImplementedInterfaces();
                }
            }

            //第四步：创建一个真正的AutoFac的工作容器  
            var container = builder.Build();
            //我们已经创建了指定程序集的所有类的对象实例，并以其接口的形式保存在AutoFac容器内存中了。那么我们怎么去拿它呢？  
            //从AutoFac容器内部根据指定的接口获取其实现类的对象实例  
            //假设我要拿到IsysFunctionServices这个接口的实现类的对象实例，怎么拿呢？  
            //var obj = container.Resolve<IsysFunctionServices>(); 
            //只有有特殊需求的时候可以通过这样的形式来拿。一般情况下没有必要这样来拿，
            //因为AutoFac会自动工作（即：会自动去类的带参数的构造函数中找与容器中key一致的参数类型，并将对象注入到类中，其实就是将对象赋值给构造函数的参数）  

            //第五步：将当前容器中的控制器工厂替换掉MVC默认的控制器工厂。（即：不要MVC默认的控制器工厂了，用AutoFac容器中的控制器工厂替代）
            //此处使用的是将AutoFac工作容器交给MVC底层 (需要using System.Web.Mvc;)  
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

    }
}