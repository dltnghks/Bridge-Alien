using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleObjectManager : MonoBehaviour
{
    private bool _isInitialize = false;
    public bool IsInitialize { get { return _isInitialize; } }
    
    private float moveSpeed = .0f;
    
    public void Initialize(float speed = .0f)
    {
        ChangeSpeed(speed);
        
        _isInitialize = true;
    }

    private void Update()
    {
        if (!_isInitialize) return;
        
        transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
    }

    public void ChangeSpeed(float speed = .0f)
    {
        this.moveSpeed = speed;
    }
}
