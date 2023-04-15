using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class DamageGiver : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            IDamagable damagable = other.collider.GetComponentInParent<IDamagable>();

            damagable?.DoDamage(100);
        }
    }
}
