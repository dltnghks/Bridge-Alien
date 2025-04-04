using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private List<GroundPositionPicker> groundPositionPickers;

    private void Awake()
    {
        
    }
}
