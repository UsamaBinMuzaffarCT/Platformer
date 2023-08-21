using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExtraAttack : MonoBehaviour
{
   public void AttackCompleted()
    {
        Destroy(gameObject);
    }
}
