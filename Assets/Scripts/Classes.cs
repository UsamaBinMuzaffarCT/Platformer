using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classes : MonoBehaviour
{
    [Serializable]
    public class FactionAnimator
    {
        public Enumirators.Faction faction;
        public RuntimeAnimatorController controller;
    }

    [Serializable]
    public class BossExtras
    {
        public GameObject extraAttackEffect;
        public Enumirators.Boss boss;
    }
}
