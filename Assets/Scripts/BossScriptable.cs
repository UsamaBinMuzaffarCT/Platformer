using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossExtrasScriptable", menuName = "ScriptableObjects/BossExtrasScriptable", order = 1)]

public class BossScriptable : ScriptableObject
{
    public List<Classes.BossExtras> extras;
}
