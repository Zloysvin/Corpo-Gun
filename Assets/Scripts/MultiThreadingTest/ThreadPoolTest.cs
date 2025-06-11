using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadPoolTest
{
    // This code works in parallel in Unity!

    private List<Task> tasks;
    public void Execute()
    {
        tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Factory.StartNew(Test));
        }
    }

    private void Test()
    {
        Thread.Sleep(1000);
        Debug.Log("Some Weird Stuff");
    }
}
