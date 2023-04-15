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
        public static Vector3 GetRandomVector3(float min, float max, bool x = true, bool y = true, bool z = true)
        {
            return new Vector3(x ? Random.Range(min, max) : 0, y ? Random.Range(min, max) : 0, z ? Random.Range(min, max) : 0);
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
        public static Vector3 GetRandomPositionInTorus(float innerRadius, float outerRadius)
        {
            float wallRadius = (outerRadius - innerRadius) * 0.5f;
            float ringRadius = wallRadius + innerRadius; // ( ( max - min ) / 2 ) + min
            // get a random angle around the ring
            float rndAngle = Random.value * 6.28f; // use radians, saves converting degrees to radians

            // determine position
            float cX = Mathf.Sin(rndAngle);
            float cZ = Mathf.Cos(rndAngle);

            Vector3 ringPos = new Vector3(cX, 0, cZ);
            ringPos *= ringRadius;

            // At any point around the center of the ring
            // a sphere of radius the same as the wallRadius will fit exactly into the torus.
            // Simply get a random point in a sphere of radius wallRadius,
            // then add that to the random center point
            Vector3 sPos = Random.insideUnitSphere * wallRadius;

            return (ringPos + sPos);
        }
    }
}
