// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class CarCrushBuilder : HurdleBuilder
// {
//     public GameObject entryPrefab;
//     public GameObject mainPrefab;
//     public GameObject endPrefab;
//
//     public override GameObject CreateEntry(Vector3 pos)
//     {
//         if (entryPrefab == null) return null;
//         
//         return Instantiate(entryPrefab, pos, Quaternion.identity);
//     }
//
//     public override GameObject CreateMain(Vector3 pos)
//     {
//         if(mainPrefab == null) return null;
//         
//         return Instantiate(mainPrefab, pos, Quaternion.identity);
//     }
//
//     public override GameObject CreateEnd(Vector3 pos)
//     {
//         if(endPrefab == null) return null;
//         
//         return Instantiate(endPrefab, pos, Quaternion.identity);
//     }
// }
