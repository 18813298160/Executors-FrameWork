using System;
 
namespace Executors {

	/// <summary>
	/// Callable object that returns type T, and may throw an exception.
	/// WARNING: Do not make Unity calls from a potentially threaded work task.
	/// Unity is generally not thread-safe.
	/// 这个接口声明call(), 可以在这个方法里实现任务的具体逻辑操作。
	/// </summary>
	public interface ICallable<T> {
		T Call();
	}
}