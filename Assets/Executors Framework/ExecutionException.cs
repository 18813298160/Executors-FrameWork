using System;
 
namespace Executors 
{
	/// <summary>
	/// Wrapper exception type for exceptions thrown during
	/// execution of an ICallable.
	/// </summary>
	public class ExecutionException : Exception 
    {
 
		public readonly Exception delayedException;
 
		public ExecutionException(Exception delayedException) {
			this.delayedException = delayedException;
		}
	}
}