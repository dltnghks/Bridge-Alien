using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoolingTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _timerText;


    private void Start()
    {
        if (_timerText == null)
        {
            _timerText = GetComponentInChildren<TextMeshPro>();
            if(_timerText == null)
            {
                Debug.LogError("Timer Text component not found in children of this object!");
            }
        }
    }

    public void SetTimerText(float time)
    {
        if (_timerText != null)
        {
            // 초:밀리초 형식으로 출력
            int seconds = Mathf.FloorToInt(time);
            int milliseconds = Mathf.FloorToInt((time - seconds) * 100);
            _timerText.text = $"{seconds:D2}:{milliseconds:D2}";
        }
    }
}
