using UnityEngine;
using Executors;
using System.Collections.Generic;

/*
 通过Executor的submit()提交Callable任务执行，将会返回Future对象。利用Future对象可以：

1.使用isDone()方法，查看任务是否完成。

2.通过GetResult()方法获call()方法执行的返回值，这个方法会一直等待callable
对象执行完call()方法并返回结果。如果出现中断，GetResult()方法会抛出Exception，或者call()方法抛出的异常。
*/

/// <summary>
/// Executor并发（多线程）框架
/// </summary>
public class ExecuteTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        IExecutor executor = new ImmediateExecutor();
        List<Future<int>> list = new List<Future<int>>();
        for (int i = 0; i < 6; i++)
        {
            var future = executor.Submit(new MultiplyIntsTask(i, 5));
            list.Add(future);
        }

        foreach (var f in list)
        {
            try
            {
                if (f.IsDone)
                    Debug.Log(f.GetResult());   
            }
            catch(ExecutionException ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        executor.Shutdown();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


class MultiplyIntsTask : ICallable<int>
{
	int a;
	int b;
	public MultiplyIntsTask(int a, int b)
	{
		this.a = a;
		this.b = b;
	}
	public int Call()
	{
		return a * b;
	}
}
