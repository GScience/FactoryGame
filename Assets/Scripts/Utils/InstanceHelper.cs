using System;
using UnityEngine;

public class InstanceHelper<T> where T : UnityEngine.Object
{
    private static T _instance;

    public static T GetGlobal()
    {
        if (_instance == null)
            _instance = UnityEngine.Object.FindObjectOfType<T>();

        return _instance;
    }

    public T Get()
    {
        return GetGlobal();
    }

    public void Set(T obj)
    {
        _instance = obj;
    }

    public static implicit operator T(InstanceHelper<T> instance)
    {
        return instance.Get();
    }

    public InstanceHelper(T instance)
    {
        Set(instance);
    }
}
