using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour, IClickable
{

    public virtual void OnClick()
    {
        Debug.Log($"{name} clicked!");
    }
}
