using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesScriptable", menuName = "ScriptableObjects/EnemiesScriptable", order = 1)]

public class EnemiesScriptable : ScriptableObject
{
    public List<Classes.Enemy> enemies;
}
