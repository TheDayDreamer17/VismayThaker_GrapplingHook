using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autovrse
{
    public class GrapplingGun : Weapon
    {
        private Player _cachedPlayerReference;
        [SerializeField] private LayerMask _canGrappleLayer;
        [SerializeField] private LineRenderer _grappleLine;
        [SerializeField] private float _ropeSpeed = 10f;
        [SerializeField] private Vector2 _minMaxDistanceMultiplier = new Vector2(0.2f, 0.9f);
        [SerializeField] private float _dampness = 7f, _massScale = 4.5f, _spring = 4.5f;
        private Transform _currentGrappledObject = null;
        private SpringJoint _playerSpringJoint;
        private Vector3 _grapplePoint;
        private float _distanceFromGrapplePoint;
        private bool _drawRope = false;
        private Coroutine _drawLineCoroutine = null;
        private Camera _mainCam;
        private void Start()
        {
            _mainCam = Camera.main;
        }
        protected override void Shoot()
        {
            Ray ray = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (_weaponData.BulletPrefab != null)
            {
                GameObject bullet = Instantiate(_weaponData.BulletPrefab, _shootPoint.transform.position, _shootPoint.transform.rotation);
                bullet.GetComponent<Rigidbody>().AddForce(ray.direction * 10 * _weaponData.Range, ForceMode.Impulse);
            }

            if (Physics.Raycast(ray, out _hit, _weaponData.Range, _canGrappleLayer))
            {
                GetObjectFromHit();
            }
            else if (Physics.SphereCast(_mainCam.transform.position, 2, ray.direction, out _hit, _weaponData.Range, _canGrappleLayer))
            {
                GetObjectFromHit();
            }


            if (_currentGrappledObject != null)
            {
                UpdateWeaponProperties();
                ChangePlayerProperties(true);
                CreateRope(() =>
                {

                });
                _playerSpringJoint = _cachedPlayerReference.GetComponent<SpringJoint>();
                if (_playerSpringJoint == null)
                    _playerSpringJoint = _cachedPlayerReference.gameObject.AddComponent<SpringJoint>();

                // _playerSpringJoint.connectedBody = _currentGrappledObject;
                _playerSpringJoint.autoConfigureConnectedAnchor = false;
                _playerSpringJoint.connectedAnchor = _grapplePoint;

                _distanceFromGrapplePoint = Vector3.Distance(_grapplePoint, _playerSpringJoint.transform.position);

                _playerSpringJoint.maxDistance = _distanceFromGrapplePoint * _minMaxDistanceMultiplier.y;
                _playerSpringJoint.minDistance = _distanceFromGrapplePoint * _minMaxDistanceMultiplier.x;

                _playerSpringJoint.spring = _spring;
                _playerSpringJoint.damper = _dampness;
                _playerSpringJoint.massScale = _massScale;
            }
        }

        // Stop player movement when grappled 
        private void ChangePlayerProperties(bool doChange)
        {
            _cachedPlayerReference.PlayerMovement.ModifyPlayerMovementState(doChange ? PlayerMovementState.Swinging : PlayerMovementState.Walking);
        }

        // Get grappled object data from raycast or spherecast hit
        private void GetObjectFromHit()
        {

            _grapplePoint = _hit.point;
            _currentGrappledObject = _hit.collider.transform;
        }

        private void CreateRope(Action OnComplete = null)
        {

            _drawLineCoroutine = StartCoroutine(DrawRope(OnComplete));
        }
        private void LateUpdate()
        {
            if (_drawRope && _currentGrappledObject != null)
            {
                transform.LookAt(_grapplePoint);
                _grappleLine.SetPosition(0, _shootPoint.position);
                _grappleLine.SetPosition(1, _grapplePoint);
            }
        }
        // It draws rope from gun to grapled point
        IEnumerator DrawRope(Action OnComplete = null)
        {
            _drawRope = true;
            _grappleLine.positionCount = 2;
            Vector3 finalPoint = _shootPoint.position;
            while (Vector3.Distance(finalPoint, _grapplePoint) > 0.05f)
            {
                finalPoint = Vector3.Lerp(finalPoint, _grapplePoint, Time.deltaTime * _ropeSpeed);
                // TODO rope animation 
                // finalPoint.y += Mathf.Sin(Time.time * _ropeSpeed * 10);

                transform.LookAt(_grapplePoint);
                _grappleLine.SetPosition(0, _shootPoint.position);
                _grappleLine.SetPosition(1, finalPoint);
                yield return null;
            }
            OnComplete?.Invoke();
            yield return null;

        }
        private void DestroyRope()
        {
            _drawRope = false;
            _currentGrappledObject = null;
            if (_drawLineCoroutine != null)
            {
                StopCoroutine(_drawLineCoroutine);
                StartCoroutine(ResetGun());
            }
            _drawLineCoroutine = null;
            _grappleLine.positionCount = 0;
        }

        IEnumerator ResetGun()
        {
            while (Vector3.Distance(transform.localEulerAngles, Vector3.zero) > 0.05f)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, 10 * Time.deltaTime);
                yield return null;
            }
        }

        public override void OnPick(Player player)
        {
            base.OnPick(player);
            _cachedPlayerReference = player;
        }
        public override void OnFireStop()
        {
            base.OnFireStop();
            ChangePlayerProperties(false);
            DestroyRope();
            if (_playerSpringJoint != null)
                Destroy(_playerSpringJoint);
            _playerSpringJoint = null;
        }
    }
}
