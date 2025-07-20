using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMap : MonoBehaviour
{
    [SerializeField] private HurdleSpawner hurdleSpawner;
    [SerializeField] private InfiniteMap infiniteMap;

    public void Initialize(MapPacket packet)
    {
        hurdleSpawner.Initialize();
        infiniteMap.Initialize(packet.maxDist, packet.onUpdateDistance);
        Debug.Log("맵 초기화 종료");
    }
}