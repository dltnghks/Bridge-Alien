using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySafeArea : MonoBehaviour
{
    [SerializeField] Vector2 margin = new Vector2(0f, 0f); // 내부 여백

    // Start is called before the first frame update
    void Start()
    {
        ApplySafeAreaWithMargin();
    }

    void ApplySafeAreaWithMargin()
    {
        Rect safe = Screen.safeArea;

        // margin 적용
        safe.xMin += margin.x;
        safe.xMax -= margin.x;
        safe.yMin += margin.y;
        safe.yMax -= margin.y;

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

}
