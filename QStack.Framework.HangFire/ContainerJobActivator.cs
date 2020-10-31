using Hangfire;
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace QStack.Framework.HangFire
{
    public class ContainerJobActivator : JobActivator
    {
        private IServiceProvider _container;

        public ContainerJobActivator(IServiceProvider container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            var implementService = _container.GetService(type);
            if(implementService==null)
            {
                var assm = type.Assembly;
                var assemblyLoadContext=  AssemblyLoadContext.All.SingleOrDefault(a => a.Assemblies.Any(a => a == assm));
                var method = assemblyLoadContext?.GetType().GetMethod("ConfigureServices");
                if(method!=null)
                {
                    var services = _container.GetService<IServiceCollection>();
                    var newService=(IServiceCollection) method.Invoke(assemblyLoadContext, new object[] { services,_container });
                    implementService = newService.BuildServiceProvider().GetService(type);
                }
            }

            return implementService;
        }
    }
}
