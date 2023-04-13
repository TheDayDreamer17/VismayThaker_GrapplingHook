using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autovrse
{
    public class FpsCamera : MonoBehaviour
    {
        [SerializeField] private float _sensiX = 25, _sensiY = 25;

        [SerializeField] private Transform _playerReference;
        private Quaternion _newRotation;
        float xRotationValue, yRotationValue;
        private bool _isUsingUI = false;
        [SerializeField] private float _minAngle = -90, _maxAngle = 90;
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void OnEnable()
        {
            PlayerInputManager.OnLookDirectionChangeActionFired += OnLookDirectionChangeActionFired;
            GameEvents.OnUIStateChanged += OnInventoryUIStateChanged;

        }
        private void OnDisable()
        {
            PlayerInputManager.OnLookDirectionChangeActionFired -= OnLookDirectionChangeActionFired;
            GameEvents.OnUIStateChanged -= OnInventoryUIStateChanged;
        }

        private void OnInventoryUIStateChanged()
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            _isUsingUI = !_isUsingUI;
        }

        private void OnLookDirectionChangeActionFired(Vector2 deltaChange)
        {
            if (_isUsingUI)
                return;
            // Get Mouse axis input data
            float mouseXValue = deltaChange.x * _sensiX;
            float mouseYValue = deltaChange.y * _sensiY;

            yRotationValue += mouseXValue;
            xRotationValue -= mouseYValue;

            xRotationValue = Mathf.Clamp(xRotationValue, _minAngle, _maxAngle);

            transform.rotation = Quaternion.Euler(xRotationValue, yRotationValue, 0);

            _playerReference.rotation = Quaternion.Euler(0, yRotationValue, 0);
        }


    }
}
