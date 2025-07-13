using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HurdleBuilder : ScriptableObject
{
    protected Transform uiParent;
    protected Transform objParent;

    public abstract GameObject CreateEntry(Vector3 position);
    public abstract GameObject CreateMain(Vector3 position);
   public abstract GameObject CreateEnd(Vector3 position);

   public void Initialize(Transform uiParent, Transform objParent)
   {
       this.uiParent = uiParent;
       this.objParent = objParent;
   }

    public void Build(params Vector3[] origins)
    {
        CreateEntry(origins[0]);

        foreach (var origin in origins)
            CreateMain(origin);
        
        CreateEnd(origins[0]);
    }
}