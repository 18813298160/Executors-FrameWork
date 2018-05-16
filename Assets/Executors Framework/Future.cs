using System;
using System.Collections.Generic;
using System.Threading;
 
namespace Executors
{
 
	public interface IFuture 
    {
		bool IsDone { get; }
	}
 
	/// <summary>
	/// 代表异步的计算结果.
    /// 如果计算出错，调用GetResult()时会报异常。
	/// </summary>
	public class Future<T> : IFuture
    {
 
		private T result;
		private Exception exception = null;
 
		volatile bool isDone = false;
		/// <summary>
		/// Is the computation done?
		/// </summary>
		public bool IsDone {
			get { return isDone; }
		}
 
 
		internal void SetResult(T result) {
			this.result = result;
		}
 
		internal void SetException(Exception e) {
			exception = e;
		}
 
		internal void SetDone() {
			isDone = true;
		}
 
 
		/// <summary>
		/// Get the result of the computation.
		/// Blocks until the computation is done.
		/// </summary>
		public T GetResult() {
			// Could maybe do this with monitor instead.
			while(!IsDone) {
				Thread.Sleep(1);
			}
 
			if(exception != null) {
				throw exception;
			}
 
			return result;
		}
	}
}