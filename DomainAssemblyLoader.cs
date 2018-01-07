using System;
using System.Reflection;

namespace Asparlose.Plugin
{
    class DomainAssemblyLoader : MarshalByRefObject
    {
        private IAssemblyLoader loader;
        public void Init(IAssemblyLoader loader)
        {
            this.loader = loader;
            AppDomain.CurrentDomain.AssemblyResolve += Domain_AssemblyResolve;
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            return Assembly.Load(loader.LoadAssembly(assemblyName));
        }

        private Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Load(new AssemblyName(args.Name));
        }
    }
}
