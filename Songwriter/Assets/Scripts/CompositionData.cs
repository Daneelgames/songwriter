using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewComposition", menuName = "GameData/NewCompositionData", order = 1)]
public class CompositionData : ScriptableObject
{
    public List<Bar> Bars = new List<Bar>(4);
}
