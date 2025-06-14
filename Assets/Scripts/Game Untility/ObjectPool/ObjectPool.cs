using UnityEngine;
using System.Collections.Generic;

/*
 * ObjectPool manages pool of reusable objects in Unity
 * decreases instantiation overhead
 */

public class ObjectPool
{
    private GameObject parent;
    private readonly PoolableObject prefab;
    private readonly int size;
    private List<PoolableObject> availableObjectsPool;
    private static Dictionary<PoolableObject, ObjectPool> ObjectPools = new();

    private ObjectPool(PoolableObject prefabIn, int sizeIn)
    {
        prefab = prefabIn;
        size = sizeIn;
        availableObjectsPool = new List<PoolableObject>(sizeIn);
    }

    // Create or use a prexisting object pool of a specific prefab
    public static ObjectPool CreateInstance(PoolableObject prefabToInstance, int poolSize)
    {
        ObjectPool pool;

        if (ObjectPools.ContainsKey(prefabToInstance))
        {
            pool = ObjectPools[prefabToInstance];
        }
        else
        {
            pool = new ObjectPool(prefabToInstance, poolSize);

            pool.parent = new GameObject(prefabToInstance + " Pool");
            pool.CreateObjects();

            ObjectPools.Add(prefabToInstance, pool);
        }


        return pool;
    }

    private void CreateObjects()
    {
        for (int i = 0; i < size; i++)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
        poolableObject.parent = this;
        poolableObject.gameObject.SetActive(false);
    }

    // Get an object from the pool if available, otherwise create a new one
    // and return it with the specified position and rotation
    public PoolableObject GetObject(Vector3 Position, Quaternion Rotation)
    {
        if (availableObjectsPool.Count == 0)
        {
            CreateObject();
        }

        PoolableObject instance = availableObjectsPool[0];

        availableObjectsPool.RemoveAt(0);

        instance.transform.SetPositionAndRotation(Position, Rotation);
        instance.gameObject.SetActive(true);

        return instance;
    }

    // Get an object from the pool with default position and rotation
    public PoolableObject GetObject()
    {
        return GetObject(Vector3.zero, Quaternion.identity);
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        availableObjectsPool.Add(Object);
    }
}
