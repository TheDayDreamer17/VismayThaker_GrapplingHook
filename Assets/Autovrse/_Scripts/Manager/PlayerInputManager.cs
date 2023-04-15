using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Autovrse
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerControls _playerControls;

        // Defining Movement Events For player
        public static Action<Vector2> OnMovementActionFired;
        public static Action OnJumpActionFired;
        public static Action<Vector2> OnLookDirectionChangeActionFired;
        private InputAction _movement, _jump, _look, _throwWeapon, _fireWeapon;
        public void Awake()
        {
            // Create instance of playercontrols to get input
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _movement = _playerControls.PlayerMovement.Movement;
                _jump = _playerControls.PlayerMovement.Jump;
                _look = _playerControls.PlayerMovement.Look;
                _throwWeapon = _playerControls.PlayerMovement.Throw_Weapon;
                _fireWeapon = _playerControls.PlayerMovement.FireWeapon;
            }
        }
        private void OnEnable()
        {
            if (_playerControls == null)
                return;
            // Enable player actions and bind events on action performed 
            _movement.performed += OnMovementPerformed;
            _movement.Enable();

            _jump.performed += OnJumpPerformed;
            _jump.Enable();

            _look.performed += OnLookDirectionChanged;
            _look.Enable();

            _throwWeapon.performed += OnWeaponThrowFired;
            _throwWeapon.Enable();

            _fireWeapon.performed += OnWeaponFired;
            _fireWeapon.canceled += OnWeaponFireStopped;
            _fireWeapon.Enable();

        }

        private void OnDisable()
        {
            if (_playerControls == null)
                return;
            // Disable player actions and unbind events on action performed
            _movement.performed -= OnMovementPerformed;
            _movement.Disable();

            _jump.performed -= OnJumpPerformed;
            _jump.Disable();

            _look.performed -= OnLookDirectionChanged;
            _look.Disable();

            _throwWeapon.performed -= OnWeaponThrowFired;
            _throwWeapon.Disable();

            _fireWeapon.performed -= OnWeaponFired;
            _fireWeapon.canceled -= OnWeaponFireStopped;
            _fireWeapon.Disable();


        }

        private void OnWeaponFired(InputAction.CallbackContext obj)
        {
            GameEvents.NotifyOnWeaponFireTriggered();
        }
        private void OnWeaponFireStopped(InputAction.CallbackContext obj)
        {
            GameEvents.NotifyOnWeaponFireStopped();
        }

        private void OnWeaponThrowFired(InputAction.CallbackContext action)
        {
            Debug.Log("G pressed");
            GameEvents.NotifyOnCurrentWeaponDropped();
        }

        private void OnLookDirectionChanged(InputAction.CallbackContext deltaChange)
        {
            OnLookDirectionChangeActionFired?.Invoke(deltaChange.ReadValue<Vector2>());
        }

        private void OnMovementPerformed(InputAction.CallbackContext moveData)
        {
            OnMovementActionFired?.Invoke(moveData.ReadValue<Vector2>());
        }

        private void OnJumpPerformed(InputAction.CallbackContext jumpData)
        {
            if (jumpData.ReadValueAsButton())
                OnJumpActionFired?.Invoke();
        }
    }
}
