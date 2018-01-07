using System.Reflection;

namespace Asparlose.Plugin
{
    /// <summary>
    /// 内部で使用する型です。
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// 指定されたアセンブリ名のイメージを読み込みます。
        /// </summary>
        /// <param name="assemblyName">アセンブリ名。</param>
        /// <returns>イメージの<see cref="byte"/>配列。</returns>
        byte[] LoadAssembly(AssemblyName assemblyName);
    }
}
