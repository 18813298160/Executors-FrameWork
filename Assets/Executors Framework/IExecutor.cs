using System;
 
namespace Executors {
 
	/// <summary>
	/// Common interface for all executors that can execute tasks.
	/// Tasks are also known as ICallable objects.
	/// </summary>
    public interface IExecutor
    {
		Future<T> Submit<T>(ICallable<T> callable);
		bool IsShutdown();
		void Shutdown();
		int GetQueueSize();
	}
 
 
	/// <summary>
	/// Optional shutdown mode specified when creating certain
	/// types of executors. Note that this is not applicable
	/// to immediate executors.
	/// Default is FinishAll.
	/// </summary>
	public enum ShutdownMode {
		FinishAll,
		CancelQueuedTasks
	}
}