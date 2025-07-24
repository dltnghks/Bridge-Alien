using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HurdleBuilder : ScriptableObject
{
    protected Transform uiParent;
    protected Transform objParent;

    public abstract GameObject CreateEntry(Transform position);
    public abstract GameObject CreateMain(Transform position); 
    public abstract GameObject CreateEnd(Transform position);

   public void Initialize(Transform uiParent, Transform objParent)
   {
       this.uiParent = uiParent;
       this.objParent = objParent;
   }

    public void Build(params Transform[] origins)
    {
        CreateEntry(origins[0]);

        foreach (var origin in origins)
        {
            if (origin == null) break;
            CreateMain(origin);
        }

        CreateEnd(origins[0]);
    }
}