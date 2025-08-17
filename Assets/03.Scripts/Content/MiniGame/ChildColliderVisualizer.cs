using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildColliderVisualizer : MonoBehaviour
{
    #if UNITY_EDITOR
    public Color gizmoColor = new Color(0, 1, 0, 0.5f);

    // 하위 오브젝트 콜리전 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Collider[] colliders3D = GetComponentsInChildren<Collider>(true);

        foreach (var col in colliders3D)
        {
            // 비활성화된 콜리전이거나 바닥인 경우 패쓰
            if (!col.enabled ||
                col.CompareTag("Ground")
            ) continue;

            Gizmos.matrix = col.transform.localToWorldMatrix;

            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
            else if (col is CapsuleCollider capsule)
            {
                Gizmos.DrawWireCube(capsule.center, new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2));
            }
            else if (col is MeshCollider mesh)
            {
                if (mesh.sharedMesh != null)
                {
                    Gizmos.DrawWireMesh(mesh.sharedMesh);
                }
            }

        }

    }

    #endif
}