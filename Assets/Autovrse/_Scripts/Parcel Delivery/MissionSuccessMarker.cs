using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{

    public class MissionSuccessMarker : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name, other.gameObject);
            if (other.transform.parent.CompareTag("Player"))
            {
                GameEvents.NotifyOnPlayerSuccessful();

            }
        }
    }
}
