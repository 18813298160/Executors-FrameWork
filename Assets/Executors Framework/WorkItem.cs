using System;
 
namespace Executors {
 
	/// <summary>
	/// Internal type used by executors to associate Future objects with
	/// callables, and to call the callable and set appropriate fields
	/// in the Future object.
	/// The non-generic interface is needed for the Executor code.
	/// </summary>
	internal interface IWorkItem {
		void Execute();
		void Cancel(string reason);
	}
 
	internal class WorkItem<T> : IWorkItem {
		public readonly ICallable<T> callable;
		public readonly Future<T> future;
 
		public WorkItem(ICallable<T> callable, Future<T> future) {
			this.callable = callable;
			this.future = future;
		}
 
		public void Execute() {
			try {
				T result = callable.Call();
				future.SetResult(result);
			} catch(Exception e) {
				future.SetException(new ExecutionException(e));
			} finally {
				future.SetDone();
			}
		}
 
		public void Cancel(string reason) {
			if(future.IsDone) {
				throw new InvalidOperationException("Can not cancel a future that is done.");
			}
			future.SetException(new ExecutionException(new Exception("Task was cancelled due to: " + reason)));
			future.SetDone();
		}
	}
}