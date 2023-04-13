using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Autovrse
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AudioSource))]
    public class Enemy : MonoBehaviour, IDamagable
    {
        // random movement speed between min and max 
        [SerializeField] private float _minSpeed = 1, _maxSpeed = 2;
        // This is the distance till it follows then jump attacks
        [SerializeField] private float _minFollowDistance = 3;
        [SerializeField] private float _damageAmount = 20;
        private bool _isActive = true;
        private Player _player;
        private Rigidbody _rb;
        private float _health = 100;
        public float Heath => _health;
        private AudioSource _audioSource;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            _player = GameObject.FindObjectOfType<Player>();

            if (_player == null)
                _isActive = false;

            _audioSource.Play();
            StartCoroutine(FindAndShootPlayer());
        }

        IEnumerator FindAndShootPlayer()
        {
            float deltaChange = 0;
            Vector3 distanceVector;
            while (_isActive)
            {

                distanceVector = _player.transform.position - transform.position;
                if (distanceVector.magnitude < _minFollowDistance)
                {
                    // jump attack on player 
                    _rb.AddForce((Vector3.up * 3) + (distanceVector * 3), ForceMode.Impulse);
                    _audioSource.Stop();
                    yield return new WaitForSeconds(1f);
                    KillEnemy();
                    yield break;
                }
                else
                {
                    transform.LookAt(_player.transform);
                    deltaChange += Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, deltaChange * Random.Range(_minSpeed, _maxSpeed));
                }
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }

        private void OnCollisionEnter(Collision other)
        {
            IDamagable damagable = other.collider.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.DoDamage(_damageAmount);

            }
        }

        public void KillEnemy()
        {
            StartCoroutine(ScaleDownAndDestroy());
        }

        IEnumerator ScaleDownAndDestroy()
        {
            while (transform.localScale.magnitude != 0)
            {
                transform.localScale /= 2;
                if (transform.localScale.x < 0.1)
                {
                    Destroy(gameObject);
                }
                yield return new WaitForSeconds(0.03f);
            }
        }

        public void DoDamage(float value)
        {
            _health -= value;
            if (_health <= 0)
                KillEnemy();
        }
    }
}
