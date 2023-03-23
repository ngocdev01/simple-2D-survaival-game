using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;


public class ObjectPool : SingletonObject<ObjectPool>
{
    Dictionary<string, ObjectPool<GameObject>> prefabPoolist = new Dictionary<string, ObjectPool<GameObject>>();
    Dictionary<string, int> poolUserCounter = new Dictionary<string, int>();

    public ObjectPool<GameObject> TryGetPool(string guid,GameObject prefab)
    {
      
        if(prefabPoolist.TryGetValue( guid, out ObjectPool<GameObject> pool))
        {
            poolUserCounter[guid]++;
            return pool;
        }
        return CreatePrefabPool(guid,prefab);
    }

    public ObjectPool<GameObject> CreatePrefabPool(string guid,GameObject prefab) 
    {
        GameObject poolHolder = new GameObject($"{gameObject.name} pool");
        poolHolder.transform.parent = this.gameObject.transform;
        var pool = new ObjectPool<GameObject>(() =>CreatePooledItem(prefab,poolHolder.transform),
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        prefabPoolist.Add(guid, pool);
        poolUserCounter.Add(guid, 1);
        return pool;
    }

    public void RequestDisposePool(string guid)
    {
        if(poolUserCounter.TryGetValue(guid,out int value))
        {
            if (value <= 1)
                prefabPoolist[guid].Dispose();
            else
                poolUserCounter[guid]--;
        }
    }

    GameObject CreatePooledItem(GameObject gameObject,Transform parent)
    {
 
        return Instantiate(gameObject, transform.position, Quaternion.identity, parent);
    }
    void OnReturnedToPool(GameObject obj)
    {
        obj.SetActive(false);
        
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(GameObject obj)
    {
       
        obj.SetActive(true);
        

    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }

}


