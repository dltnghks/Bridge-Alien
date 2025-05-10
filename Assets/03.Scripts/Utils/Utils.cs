using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class Utils
{
    public static T ParseEnum<T>(String value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    // System.Type을 받는 버전
    public static Component GetOrAddComponent(GameObject go, System.Type componentType)
    {
        if (go == null || componentType == null) return null;
        if (!typeof(Component).IsAssignableFrom(componentType)) // Component를 상속하는 타입인지 확인
        {
            Debug.LogError($"{componentType.Name} is not a Component type.");
            return null;
        }
        Component component = go.GetComponent(componentType);
        if (component == null)
        {
            component = go.AddComponent(componentType);
        }
        return component;
    }

    
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
        {
            return null;
        }

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
            {
                return transform.GetComponent<T>();
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
        {
            return transform.gameObject;
        }

        return null;
    }
}