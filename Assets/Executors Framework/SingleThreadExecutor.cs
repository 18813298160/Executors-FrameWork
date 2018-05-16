using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
 
namespace Executors {
 
	/// <summary>
	/// Single threaded executor. Useful for asynchronous operations
	/// without making the program overly complex.
	/// </summary>
	class SingleThreadExecutor : IExecutor {
		private Thread workerThread = null;
		private readonly Queue<IWorkItem> taskQueue = new Queue<IWorkItem>();
		private readonly object locker = new object();
 
		private ShutdownMode shutdownMode;
		volatile bool shutdown = false;
		volatile bool shutdownCompleted = false;
 
 
		public SingleThreadExecutor() : this(ShutdownMode.FinishAll) { }
 
		public SingleThreadExecutor(ShutdownMode shutdownMode) {
			this.shutdownMode = shutdownMode;
			ThreadStart start = new ThreadStart(RunWorker);
			workerThread = new Thread(start);
			workerThread.Start();
		}
 
 
		void RunWorker() {
			while(!shutdown) {
				lock(locker) {
					while(taskQueue.Count == 0 && !shutdown) {
						Monitor.Wait(locker);
					}
				}
 
				while(taskQueue.Count > 0) {
					bool shouldCancel = (shutdown && shutdownMode.Equals(ShutdownMode.CancelQueuedTasks));
					if(shouldCancel) {
						break;
					}
 
					IWorkItem task = null;
					lock(locker) {
						if(taskQueue.Count > 0) {
							task = taskQueue.Dequeue();
						}
					}
					if(task != null) {
						task.Execute();
					}
				}
			}
 
			foreach(IWorkItem task in taskQueue) {
				task.Cancel("Shutdown");
			}
 
			shutdownCompleted = true;
		}
 
 
		#region IExecutor Members
 
		public Future<T> Submit<T>(ICallable<T> callable) {
			lock(locker) {
				if(shutdown) {
					throw new InvalidOperationException("May not submit tasks after shutting down executor.");
				}
				Future<T> future = new Future<T>();
				WorkItem<T> task = new WorkItem<T>(callable, future);
				taskQueue.Enqueue(task);
				Monitor.Pulse(locker);
				return future;
			}
		}
 
		public bool IsShutdown() {
			return shutdownCompleted;
		}
 
		public void Shutdown() {
			lock(locker) {
				shutdown = true;
				Monitor.Pulse(locker);
			}
		}
 
		public int GetQueueSize() {
			// FIXME: Find out if lock is really necessary here.
			lock(locker) {
				return taskQueue.Count;
			}
		}
 
		#endregion
	}
}