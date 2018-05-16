using System;
 
namespace Executors {
 
	/// <summary>
	/// Non-threaded immediate executor.
	/// Mainly a convenience executor - makes it easy
	/// to switch between threaded and non-threaded approaches.
	/// </summary>
	public class ImmediateExecutor : IExecutor {
		private bool shutdown = false;
 
		#region IExecutor Members
 
		public Future<T> Submit<T>(ICallable<T> callable) {
			if(shutdown) {
				throw new InvalidOperationException("May not submit tasks after shutting down executor.");
			}
			Future<T> future = new Future<T>();
			WorkItem<T> task = new WorkItem<T>(callable, future);
			((IWorkItem)task).Execute();
			return future;
		}
 
		public bool IsShutdown() {
			return shutdown;
		}
 
		public void Shutdown() {
			shutdown = true;
		}
 
		public int GetQueueSize() {
			return 0;
		}
 
		#endregion
 
 
	}
}