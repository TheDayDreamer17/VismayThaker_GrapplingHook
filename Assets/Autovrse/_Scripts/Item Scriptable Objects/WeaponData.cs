using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autovrse
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "WeaponData", menuName = "VismayThaker_InventorySystem/WeaponData", order = 0)]
    [System.Serializable]
    public class WeaponData : ScriptableObject
    {
        public int MagazineSize = 6;
        public GameObject BulletPrefab;
        public float DropFrontForce = 5;
        public float DropUpwardForce = 5;
        public float DropTorqueForce = 20;

        public int DamageAmount = 20;
        public float TimeBetweenShooting = 1, Range = 10, ReloadTime = 1;

        public bool AllowButtonHold;

    }
}
