using UnityEngine;
using System.Collections.Generic;

public class InGameGauge : MonoBehaviour
{
    // 인스펙터에서 채워 줄 게이지 블록(칸) 오브젝트들
    public List<GameObject> gaugeBlocks;

    private void Awake()
    {
        // 시작할 때 모든 블록을 비활성화
        foreach (var block in gaugeBlocks)
        {
            block.SetActive(false);
        }
    }

    // 특정 값만큼 게이지를 채우는 메서드
    public void SetValue(int value)
    {
        // value 값에 따라 블록을 켜거나 끔
        for (int i = 0; i < gaugeBlocks.Count; i++)
        {
            // i가 value보다 작으면 블록을 활성화, 아니면 비활성화
            gaugeBlocks[i].SetActive(i < value);
        }
    }
}
