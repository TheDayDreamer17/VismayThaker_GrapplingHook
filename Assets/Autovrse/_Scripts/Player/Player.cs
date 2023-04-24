using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class Player : MonoBehaviour, IDamagable
    {
        private Rigidbody _rb;
        public Rigidbody RB => _rb;
        private PlayerWeaponController _playerWeaponController;
        public PlayerWeaponController PlayerWeaponController => _playerWeaponController;
        private PlayerMovement _playerMovement;
        public PlayerMovement PlayerMovement => _playerMovement;

        [SerializeField] private float _health = 100;
        public float Heath => _health;
        private bool _isUsingUI = false;
        public bool IsUsingUI => _isUsingUI;
        private Vector3 _initialPosition;
        private void OnEnable()
        {
            GameEvents.OnGameStart += OnGameStart;
            GameEvents.OnGameRestart += OnGameRestart;
            GameEvents.OnUIStateChanged += OnUIStateChanged;
        }
        private void OnDisable()
        {
            GameEvents.OnGameStart += OnGameStart;
            GameEvents.OnGameRestart -= OnGameRestart;
            GameEvents.OnUIStateChanged -= OnUIStateChanged;
        }

        private void OnGameStart()
        {
            _initialPosition = transform.position;
        }

        private void OnUIStateChanged()
        {
            _isUsingUI = !_isUsingUI;
            if (_isUsingUI)
            {
                _playerWeaponController?.DisableCurrentWeapon();
            }
            else
                _playerWeaponController?.EnableCurrentWeapon();
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _playerWeaponController = GetComponent<PlayerWeaponController>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void ModifyHealth(float amount)
        {
            _health = _health + amount > 100 ? 100 : _health + amount;
            GameEvents.NotifyOnPlayerHealthChanged(_health);
        }

        public void DoDamage(float damageAmount)
        {
            float newHealth = _health - damageAmount;
            _health = newHealth <= 0 ? 0 : newHealth;
            GameEvents.NotifyOnPlayerHealthChanged(_health);
            if (_health == 0)
            {
                //Player is dead
                GameEvents.NotifyOnPlayerUnSuccessful();
            }
        }

        private void OnGameRestart()
        {
            ModifyHealth(100);
            transform.position = _initialPosition;
        }
    }
}
