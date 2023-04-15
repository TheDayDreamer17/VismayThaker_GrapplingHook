using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Player))]
    public class PlayerMovement : MonoBehaviour
    {
        Player _player;
        Rigidbody _rb;
        [Header("Movement")]
        [SerializeField] private float _movementSpeed = 5;
        [SerializeField] private float _dragValueOnGround = 5f;
        private Vector2 _moveInput;
        private Vector3 _moveDirection;
        [Header("Jump")]
        [SerializeField] private float _upwardForce = 5;
        [SerializeField] private float _airMultiplier = 5;
        [SerializeField] private LayerMask _ground;
        [SerializeField] private float _jumpDistanceValue = 0.2f;
        [SerializeField] private float _playerHeight = 2;
        private bool _isInAir = false;
        private PlayerMovementState _playerMovementState = PlayerMovementState.Walking;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }
        private void Update()
        {
            if (_player.IsUsingUI)
                return;
            _isInAir = !Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + _jumpDistanceValue, _ground);
            Debug.DrawRay(transform.position, Vector3.down * (_playerHeight * 0.5f + _jumpDistanceValue), Color.green);
            if (!_isInAir)
            {
                _rb.drag = _dragValueOnGround;
            }
            else
                _rb.drag = 0;

            ControlSpeed();
        }

        public void ModifyJumpParameter(float jumpValue, float duration)
        {

            _upwardForce *= jumpValue;
            // give effect till duration seconds
            this.DoActionWithDelay(() => { _upwardForce /= jumpValue; }, duration);

        }

        public void ModifyPlayerMovementState(PlayerMovementState playerMovementState)
        {
            _playerMovementState = playerMovementState;
        }
        private void OnEnable()
        {
            // Subscribe to input managet events
            PlayerInputManager.OnMovementActionFired += OnMovementActionFired;
            PlayerInputManager.OnJumpActionFired += OnJumpActionFired;
        }
        private void OnDisable()
        {
            // UnSubscribe to input managet events
            PlayerInputManager.OnMovementActionFired -= OnMovementActionFired;
            PlayerInputManager.OnJumpActionFired -= OnJumpActionFired;
        }

        private void FixedUpdate()
        {
            CalculateMovement();
        }

        private void CalculateMovement()
        {
            // Changing position and rotation based on input
            _moveDirection = transform.forward * _moveInput.y + transform.right * _moveInput.x;
            if (!_isInAir)
                _rb.AddForce(_moveDirection.normalized * _movementSpeed, ForceMode.Force);
            else
                _rb.AddForce(_moveDirection.normalized * _movementSpeed * _airMultiplier, ForceMode.Force);
        }

        private void OnMovementActionFired(Vector2 movementData)
        {
            _moveInput = movementData;
        }

        private void OnJumpActionFired()
        {
            CalculateJump();

        }

        private void CalculateJump()
        {
            if (_isInAir)
                return;
            // reset y velocity and add upward force
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(transform.up * _upwardForce, ForceMode.Impulse);
        }

        private void ControlSpeed()
        {
            Vector3 currentVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (currentVelocity.magnitude > _movementSpeed)
            {
                Vector3 maxVelocity = currentVelocity.normalized * _movementSpeed;
                _rb.velocity = new Vector3(maxVelocity.x, _rb.velocity.y, maxVelocity.z);
            }
        }

    }

    public enum PlayerMovementState
    {
        Walking, Swinging
    }
}
