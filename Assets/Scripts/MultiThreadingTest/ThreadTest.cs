using UnityEngine;

public class ThreadTest : MonoBehaviour
{
    void Start()
    {
        ThreadPoolTest test = new ThreadPoolTest();
        test.Execute();
    }
}
