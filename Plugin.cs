using System;
using System.Collections.Generic;
using System.Threading;

namespace Asparlose.Plugin
{
    /// <summary>
    /// 新たな<see cref="AppDomain"/>内で作成したインスタンスを表します。
    /// </summary>
    /// <typeparam name="T">インスタンスの型。シリアル化可能か、リモート呼び出しできる必要があります。</typeparam>
    public sealed class Plugin<T> : IDisposable
    {
        internal Plugin(AppDomain appDomain, T instance)
        {
            this.appDomain = appDomain;
            this.instance = instance;
        }

        private readonly AppDomain appDomain;

        readonly T instance;
        /// <summary>
        /// 異なる<see cref="AppDomain"/>に読み込まれた<typeparamref name="T"/>型のインスタンス。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトは既に破棄されています。</exception>
        public T Instance => IsDisposed ? throw new ObjectDisposedException(GetType().Name) : instance;

        private int isDisposed;

        /// <summary>
        /// <see cref="Plugin{T}"/>が破棄されているかどうかを取得する。
        /// </summary>
        public bool IsDisposed => isDisposed != 0;

        /// <summary>
        /// <see cref="AppDomain"/>をアンロードします。
        /// <see cref="Instance"/>が<see cref="IDisposable"/>を継承していたら、アンロードの前にそれを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref isDisposed, 1) == 0)
            {
                if (instance is IDisposable i)
                    i.Dispose();

                AppDomain.Unload(appDomain);
            }
        }
    }
}
