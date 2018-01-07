using System;
using System.IO;
using System.Reflection;

namespace Asparlose.Plugin
{
    /// <summary>
    /// 外部のアセンブリ内の型を読み込む機能を提供します。
    /// </summary>
    /// <typeparam name="T">読み込むの派生元型。</typeparam>
    public abstract class PluginLoader<T> : MarshalByRefObject, IAssemblyLoader
    {
        /// <summary>
        /// 新たな<see cref="AppDomain"/>を作成し、<typeparamref name="T"/>型のインスタンスを生成します。。
        /// </summary>
        /// <param name="assemblyName">読み込むアセンブリの名前。</param>
        /// <param name="typeName">型の名前。</param>
        /// <returns>読み込まれたプラグインを表す<see cref="Plugin{T}"/>。</returns>
        /// <exception cref="TypeLoadException">指定した名前の型はありませんでした。</exception>
        /// <exception cref="FileNotFoundException">ファイルは見つかりませんでした。</exception>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="MissingMethodException" />
        /// <exception cref="MethodAccessException" />
        /// <exception cref="AppDomainUnloadedException" />
        /// <exception cref="BadImageFormatException" />
        /// <exception cref="FileLoadException" />
        public Plugin<T> Load(AssemblyName assemblyName, string typeName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            AppDomainSetup domainSetup = new AppDomainSetup()
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            };

            var domain = AppDomain.CreateDomain(assemblyName.Name + ":" + typeName, null, domainSetup);

            var loader = (DomainAssemblyLoader)domain.CreateInstanceAndUnwrap(typeof(DomainAssemblyLoader).Assembly.FullName, typeof(DomainAssemblyLoader).FullName);
            loader.Init(this);

            var ins = (T)domain.CreateInstanceAndUnwrap(assemblyName.ToString(), typeName);
            return new Plugin<T>(domain, ins);
        }

        byte[] IAssemblyLoader.LoadAssembly(AssemblyName assemblyName)
            => LoadAssembly(assemblyName);

        /// <summary>
        /// アセンブリの名前から、イメージを読み込みます。
        /// </summary>
        /// <param name="assemblyName">アセンブリの名前。</param>
        /// <returns>イメージの<see cref="byte"/>配列。</returns>
        protected abstract byte[] LoadAssembly(AssemblyName assemblyName);

        /// <summary>
        /// 指定されたディレクトリ以下の .dll ファイルを読み込む<see cref="PluginLoader{T}"/>を作成します。
        /// </summary>
        /// <param name="directory">ディレクトリ。nullの場合は現在のディレクトリ。</param>
        /// <returns>指定されたディレクトリ以下の .dll ファイルを読み込む<see cref="PluginLoader{T}"/>。</returns>
        public static PluginLoader<T> CreateFileLoader(DirectoryInfo directory)
            => new FromFile(directory ?? new DirectoryInfo(Environment.CurrentDirectory));

        class FromFile : PluginLoader<T>
        {
            readonly DirectoryInfo directory;

            public FromFile(DirectoryInfo directory)
            {
                this.directory = directory;
            }

            protected override byte[] LoadAssembly(AssemblyName assemblyName)
            {
                using (var s = File.OpenRead(Path.Combine(directory.FullName, assemblyName.Name + ".dll")))
                {
                    var b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    return b;
                }
            }
        }
    }
}
