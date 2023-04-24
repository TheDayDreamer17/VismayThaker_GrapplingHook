using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autovrse
{
    public class FpsCamera : MonoBehaviour
    {
        [SerializeField] private float _sensiX = 25, _sensiY = 25;
        [SerializeField] private Transform _playerCameraPosition;
        [SerializeField] private Transform _playerOrientation;
        [SerializeField] private Transform _playerBody;
        private Quaternion _newRotation;
        float xRotationValue, yRotationValue;
        private bool _isUsingUI = false;
        private bool _followPlayer = false;
        [SerializeField] private float _minAngle = -90, _maxAngle = 90;

        [Header("Camera Shake")]
        // How long the object should shake for.
        private float _shakeDuration = 0f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float ShakeAmount = 0.7f;
        public float DecreaseFactor = 1.0f;
        private Animator _animator;
        Vector3 originalPos;
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _animator = GetComponent<Animator>();
            if (_animator == null)
                Debug.LogError("animator is not present on this object");
        }
        private void OnEnable()
        {
            GameEvents.OnGameStart += OnGameStart;
            PlayerInputManager.OnLookDirectionChangeActionFired += OnLookDirectionChangeActionFired;
            GameEvents.OnUIStateChanged += OnInventoryUIStateChanged;

        }
        private void OnDisable()
        {
            GameEvents.OnGameStart -= OnGameStart;
            PlayerInputManager.OnLookDirectionChangeActionFired -= OnLookDirectionChangeActionFired;
            GameEvents.OnUIStateChanged -= OnInventoryUIStateChanged;
        }

        private void OnGameStart()
        {
            _animator.enabled = false;
            _followPlayer = true;
        }

        private void OnInventoryUIStateChanged()
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            _isUsingUI = !_isUsingUI;
        }

        private void OnLookDirectionChangeActionFired(Vector2 deltaChange)
        {
            if (_isUsingUI || !_followPlayer)
                return;
            // Get Mouse axis input data
            float mouseXValue = deltaChange.x * _sensiX;
            float mouseYValue = deltaChange.y * _sensiY;

            yRotationValue += mouseXValue;
            xRotationValue -= mouseYValue;

            xRotationValue = Mathf.Clamp(xRotationValue, _minAngle, _maxAngle);

            Quaternion newRotation = Quaternion.Euler(xRotationValue, yRotationValue, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10);
            _playerOrientation.rotation = Quaternion.Euler(0, yRotationValue, 0);
            _playerBody.rotation = Quaternion.Euler(0, yRotationValue, 0);
        }


        public void PerformCameraShake(float duration = 0.2f)
        {
            _shakeDuration = duration;
        }
        private void LateUpdate()
        {
            if (!_followPlayer)
                return;

            transform.position = Vector3.Lerp(transform.position, _playerCameraPosition.position, Time.deltaTime * 30);
            if (_shakeDuration > 0)
            {
                transform.localPosition += UnityEngine.Random.insideUnitSphere * ShakeAmount;
                _shakeDuration -= Time.deltaTime * DecreaseFactor;

            }


        }
    }
}
