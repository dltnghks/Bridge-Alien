using UnityEngine;
using UnityEditor;

public class InfiniteScrollDebugger : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public Transform[] cloudObjects;
    public Transform[] backgroundObjects;

    public Color cameraEdgeColor = Color.yellow;
    public Color cloudColor = Color.cyan;
    public Color backgroundColor = Color.green;

    private void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 1. 카메라 좌우 경계 계산
        float leftEdge = cam.ViewportToWorldPoint(Vector3.zero).x;
        float rightEdge = cam.ViewportToWorldPoint(Vector3.right).x;

        // 2. 카메라 경계 시각화
        Gizmos.color = cameraEdgeColor;
        Gizmos.DrawLine(new Vector3(leftEdge, -10, 0), new Vector3(leftEdge, 10, 0));
        Gizmos.DrawLine(new Vector3(rightEdge, -10, 0), new Vector3(rightEdge, 10, 0));
        Gizmos.DrawWireCube(new Vector3((leftEdge + rightEdge) / 2f, 0, 0), new Vector3(rightEdge - leftEdge, 20f, 0));

        // 3. Cloud 위치 & 폭 시각화
        if (cloudObjects != null)
        {
            Gizmos.color = cloudColor;
            foreach (var cloud in cloudObjects)
            {
                if (cloud == null) continue;

                SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                float width = sr.bounds.size.x;
                Vector3 pos = cloud.position;

                Gizmos.DrawWireCube(pos, new Vector3(width, 1f, 0));
                Handles.Label(pos + Vector3.up * 1.5f, $"Cloud\nx={pos.x:F1}");
            }
        }

        // 4. Background 위치 & 폭 시각화
        if (backgroundObjects != null)
        {
            Gizmos.color = backgroundColor;
            foreach (var bg in backgroundObjects)
            {
                if (bg == null) continue;

                SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                float width = sr.bounds.size.x;
                Vector3 pos = bg.position;

                Gizmos.DrawWireCube(pos, new Vector3(width, 1f, 0));
                Handles.Label(pos + Vector3.up * 1.5f, $"BG\nx={pos.x:F1}");
            }
        }
    }
}
