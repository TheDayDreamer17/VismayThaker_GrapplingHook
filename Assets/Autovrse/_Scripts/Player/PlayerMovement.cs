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

        [SerializeField] private ParticleSystem _hitParticleEfect;
        [SerializeField] private float _minHitVelocity = 10;
        private FpsCamera _fpsCamera;
        [Header("Movement")]

        [SerializeField] private Transform _playerOrientation;
        [SerializeField] private float _movementSpeed = 5;
        [SerializeField] private float _dragValueOnGround = 5f;
        [SerializeField] private float _gravitationalExtraForce = 5f;
        private Vector2 _moveInput;
        private Vector3 _moveDirection;
        [Header("Swinging")]
        [SerializeField] private float _dragValueWhileSwinging = 0;
        [SerializeField] private float _swingSpeed = 5;
        [Header("Jump")]
        [SerializeField] private float _dragValueInAir = 0.2f;
        [SerializeField] private float _upwardForce = 5;
        [SerializeField] private float _airMultiplier = 5;
        [SerializeField] private LayerMask _ground;
        [SerializeField] private float _jumpDistanceValue = 0.2f;
        [SerializeField] private float _playerHeight = 2;
        private bool _isInAir = false;
        private PlayerMovementState _playerMovementState = PlayerMovementState.Walking;
        private float _playerMovementSpeed = 0;
        private void Awake()
        {
            _player = GetComponent<Player>();
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _playerMovementSpeed = _movementSpeed;
            _fpsCamera = Camera.main.GetComponent<FpsCamera>();
        }
        private void Update()
        {
            if (_player.IsUsingUI)
                return;
            _isInAir = !Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + _jumpDistanceValue, _ground);
            // Debug.DrawRay(transform.position, Vector3.down * (_playerHeight * 0.5f + _jumpDistanceValue), Color.green);
            if (_playerMovementState == PlayerMovementState.Swinging)
            {
                _rb.drag = _dragValueWhileSwinging;
            }
            else if (!_isInAir)
            {
                _rb.drag = _dragValueOnGround;
            }
            else
                _rb.drag = _dragValueInAir;

            if (_playerMovementState == PlayerMovementState.Walking)
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
            switch (playerMovementState)
            {
                case PlayerMovementState.Walking:
                    _playerMovementSpeed = _movementSpeed;
                    break;
                case PlayerMovementState.Swinging:
                    _playerMovementSpeed = _swingSpeed;

                    break;

            }
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
            AddExtraGravityForce();
            CalculateMovement();
        }

        private void AddExtraGravityForce()
        {
            if (_isInAir && _playerMovementState == PlayerMovementState.Walking)
                _rb.AddForce(-transform.up * _gravitationalExtraForce * Time.deltaTime, ForceMode.VelocityChange);
        }

        private void CalculateMovement()
        {
            // Changing position based on input
            _moveDirection = _playerOrientation.forward * _moveInput.y + _playerOrientation.right * _moveInput.x;

            if (!_isInAir)
            {
                _rb.AddForce(_moveDirection.normalized * _playerMovementSpeed * Time.deltaTime);
            }
            else
            {
                _rb.AddForce(_moveDirection.normalized * _playerMovementSpeed * _airMultiplier * Time.deltaTime);
            }
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
            if (currentVelocity.magnitude > _playerMovementSpeed)
            {
                Vector3 maxVelocity = currentVelocity.normalized * _playerMovementSpeed;
                _rb.velocity = new Vector3(maxVelocity.x, _rb.velocity.y, maxVelocity.z);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && _rb.velocity.magnitude > _minHitVelocity)
            {
                _hitParticleEfect.transform.position = other.contacts[0].point;
                _hitParticleEfect.Play();
                _fpsCamera?.PerformCameraShake();
            }
        }

    }

    public enum PlayerMovementState
    {
        Walking, Swinging
    }
}
