using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Autovrse
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        public static T Instance => _instance;

        public virtual void Awake()
        {
            if (_instance == null)
                _instance = this as T;
            else
                Destroy(this.gameObject);
        }


    }
}
