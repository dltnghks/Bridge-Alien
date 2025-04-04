using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HurdlePattern", menuName = "Hurdle/HurdlePattern", order = 1)]
public class HurdlePattern : ScriptableObject
{
    [field:SerializeField] public List<HurdlePatternData> HurdlePatternDataList { get; private set; }
}
