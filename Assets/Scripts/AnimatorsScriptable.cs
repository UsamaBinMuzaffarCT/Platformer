using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AnimatorsScriptableObject", menuName = "ScriptableObjects/AnimatorsScriptableObject", order = 1)]
public class AnimatorsScriptable : ScriptableObject
{
    public List<Classes.FactionAnimator> factionAnimators;
}
