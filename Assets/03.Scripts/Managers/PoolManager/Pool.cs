using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    public GameObject Original { get; private set; }
    public Transform Root { get; set; }

    private Stack<Poolable> _poolStack = new Stack<Poolable>();

    public void init(GameObject original, int count = 5)
    {
        Original = original;
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";

        for (int i = 0; i < count; i++)
        {
            Push(Create());
        }
    }

    private Poolable Create()
    {
        GameObject go = UnityEngine.Object.Instantiate<GameObject>(Original);
        go.name = Original.name;
        return go.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
        if (poolable == null)
        {
            return;   
        }

        poolable.transform.SetParent(Root);
        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;
        
        _poolStack.Push(poolable);
    }

    public Poolable Pop(Transform parent)
    {
        if (parent == null)
        {
            Logger.LogWarning("Not Setting ParentTransform in Pool pop");
        }
        
        Poolable poolable;

        if (_poolStack.Count > 0)
        {
            poolable = _poolStack.Pop();
        }
        else
        {
            poolable = Create();
        }
        
        poolable.gameObject.SetActive(true);
        
        poolable.transform.SetParent(parent);
        poolable.IsUsing = true;

        return poolable;
    }
}