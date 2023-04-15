using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class Platform : MonoBehaviour, IDynamicVisibility
    {
        private bool _isFinalPlatform;
        public bool IsFinalPlatform => _isFinalPlatform;

        private bool _didPlayerVisitedThisPlatform = false;
        public bool DidPlayerVisitedThisPlatform => _didPlayerVisitedThisPlatform;
        private Player _cachedPlayerReference;
        [SerializeField] private Collider _collider;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _normalFloorMat;
        [SerializeField] private Material _finalFloorMat;
        public bool IsHidden { get; private set; }
        private void OnCollisionEnter(Collision other)
        {
            Player player = other.collider.GetComponentInParent<Player>();
            if (player != null)
            {
                if (IsFinalPlatform && !_didPlayerVisitedThisPlatform)
                {
                    PlatformSpawner.OnFinalPlatformReached?.Invoke();
                    // add bullets to gun for reaching final point
                    player.PlayerWeaponController.CurrentWeapon.OnAddBullets(5);
                }
                _cachedPlayerReference = player;
                _didPlayerVisitedThisPlatform = true;
            }
        }
        public void PutPlayerOnThisPlatform()
        {
            _cachedPlayerReference.transform.position = transform.position + Vector3.up * 4;
        }

        public void Setup(Vector3 position, bool isFinalPlatform)
        {
            StartCoroutine(GoToFinalPoint(position));
            _didPlayerVisitedThisPlatform = false;
            _cachedPlayerReference = null;
            _isFinalPlatform = isFinalPlatform;
            _meshRenderer.material = isFinalPlatform ? _finalFloorMat : _normalFloorMat;
        }

        IEnumerator GoToFinalPoint(Vector3 position)
        {
            while (Vector3.Distance(transform.position, position) > 0.04f)
            {
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 7);
                yield return null;
            }
        }

        public void Show()
        {
            _meshRenderer.enabled = true;
            _collider.enabled = true;
            IsHidden = false;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _collider.enabled = false;
            IsHidden = true;
        }
    }
}
