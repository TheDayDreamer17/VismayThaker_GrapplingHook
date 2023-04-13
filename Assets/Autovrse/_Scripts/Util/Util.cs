using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Autovrse
{
    public static class Util
    {
        public static Vector3 GetRandomVector3(float min, float max)
        {
            return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        }

        public static void ToggleCollidersArray(Collider[] colliders, bool enabled)
        {
            foreach (var item in colliders)
            {
                item.enabled = enabled;
            }
        }
        public static void ToggleCollidersArrayAtDelay(this MonoBehaviour monoBehaviour, Collider[] colliders, bool enabled, float delay = 0.2f)
        {
            monoBehaviour.StartCoroutine(ToggleCollidersArrayAtDelayCoroutine(colliders, enabled, delay));
        }
        private static IEnumerator ToggleCollidersArrayAtDelayCoroutine(Collider[] colliders, bool enabled, float delay)
        {
            yield return new WaitForSeconds(delay);
            foreach (var item in colliders)
            {
                item.enabled = enabled;
            }
            yield return null;
        }

        public static void DoActionWithDelay(this MonoBehaviour monoBehaviour, Action OnComplete, float delay)
        {
            monoBehaviour.StartCoroutine(DoActionWithDelayCoroutine(OnComplete, delay));
        }

        private static IEnumerator DoActionWithDelayCoroutine(Action OnComplete, float delay)
        {
            yield return new WaitForSeconds(delay);
            OnComplete?.Invoke();
            yield return null;
        }


        static Coroutine DoRotationShowcaseCoroutineData;
        public static void DoRotationShowcase(this MonoBehaviour monoBehaviour, Vector3 axis)
        {

            DoRotationShowcaseCoroutineData = monoBehaviour.StartCoroutine(DoRotationShowcaseCoroutine(monoBehaviour.transform, axis));
        }

        public static void StopRotationShowcase(this MonoBehaviour monoBehaviour)
        {
            if (DoRotationShowcaseCoroutineData != null)
                monoBehaviour.StopCoroutine(DoRotationShowcaseCoroutineData);
        }

        private static IEnumerator DoRotationShowcaseCoroutine(Transform objectTransform, Vector3 axis)
        {
            float angle = 0;
            while (true)
            {
                angle += 20;
                objectTransform.RotateAround(objectTransform.position, axis, angle);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
