using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SingletonObject : MonoBehaviour 
{
    
}
public class SingletonObject<T> : SingletonObject where T : SingletonObject<T>
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                T[] instances = FindObjectsOfType<T>();
                if (instances.Length>1)
                {
                    Debug.LogWarning($"There are multiple instance of {typeof(T)} is running");                  
                }
                if (instances.Length == 0)
                    throw new NullReferenceException($"Singleton {typeof(T)} has no instance");
                instance = instances[0];
            }
            return instance as T;
        }
        private set { }
    }
}
