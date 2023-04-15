using System;
using System.Collections;
using UnityEngine;
using TMPro;
namespace Autovrse
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviour, IWeaponItem
    {
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected WeaponData _weaponData;
        public WeaponData WeaponData => _weaponData;
        [SerializeField] private Collider[] _colliders;
        public Collider[] Colliders => _colliders;

        private Rigidbody _rb;
        private Transform _originalParent;
        private int _bulletsLeft;
        Coroutine _weaponFireCoroutine = null;
        protected RaycastHit _hit;
        protected bool _readyToShoot = true, _reloading = false, _canShoot = true;
        [SerializeField] private TMP_Text _bulletCountText;
        private AudioSource _audioSource;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _originalParent = transform.parent;
            _rb = GetComponent<Rigidbody>();
            _bulletsLeft = _weaponData.MagazineSize;
            _bulletCountText.text = _bulletsLeft.ToString();
        }
        IEnumerator OnFireCoroutine()
        {
            do
            {
                if (_bulletsLeft == 0)
                {
                    OnMagazineEmpty();
                    yield break;
                }
                if (!_canShoot)
                    yield break;
                if (_readyToShoot && !_reloading)
                {
                    Shoot();
                }
                yield return new WaitForSeconds(_weaponData.TimeBetweenShooting);
                _readyToShoot = true;
            } while (_weaponData.AllowButtonHold);
            _weaponFireCoroutine = null;
            yield return null;
        }

        protected virtual void UpdateWeaponProperties()
        {
            _readyToShoot = false;
            _bulletsLeft--;
            _bulletCountText.text = _bulletsLeft.ToString();
            _audioSource.Play();
        }

        protected virtual void Shoot()
        {
            UpdateWeaponProperties();
            GameObject bullet = Instantiate(_weaponData.BulletPrefab, _shootPoint.transform.position, _shootPoint.transform.rotation);
            Vector3 direction = _shootPoint.forward;
            bullet.GetComponent<Rigidbody>().AddForce(direction * 10 * _weaponData.Range, ForceMode.Impulse);
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out _hit))
            {
                IDamagable damagable = _hit.collider.GetComponent<IDamagable>();
                damagable?.DoDamage(_weaponData.DamageAmount);
            }
        }
        public virtual void OnFireStart()
        {
            if (_bulletsLeft == 0)
            {
                OnMagazineEmpty();
                return;
            }
            if (!_canShoot) return;
            _audioSource.loop = _weaponData.AllowButtonHold;
            _readyToShoot = true;
            _weaponFireCoroutine = StartCoroutine(OnFireCoroutine());
        }
        public virtual void OnFireStop()
        {
            _audioSource.Stop();
            if (_weaponFireCoroutine != null)
                StopCoroutine(_weaponFireCoroutine);
            _weaponFireCoroutine = null;
        }
        public void ReloadWeapon()
        {
            _reloading = true;
            this.DoActionWithDelay(() =>
            {
                _bulletsLeft += _weaponData.MagazineSize;
                _bulletCountText.text = _bulletsLeft.ToString();
                _reloading = false;
            }, _weaponData.ReloadTime);
        }
        private void OnMagazineEmpty()
        {
            OnFireStop();
        }

        public virtual void OnPick(Player player)
        {
            // Depending upon the gun type subscribe shooting mode
            GameEvents.OnWeaponFireTriggered += OnFireStart;
            GameEvents.OnWeaponFireStopped += OnFireStop;

            _rb.isKinematic = true;
            this.ToggleCollidersArrayAtDelay(_colliders, false);
        }

        public virtual void OnDrop(Player player)
        {
            // unsubscribe shooting mode on drop
            GameEvents.OnWeaponFireTriggered -= OnFireStart;
            GameEvents.OnWeaponFireStopped -= OnFireStop;

            // Add force to show item dropping from inventory
            _rb.isKinematic = false;
            _rb.velocity = player.RB.velocity;
            _rb.AddForce(player.transform.forward * _weaponData.DropFrontForce, ForceMode.Impulse);
            _rb.AddForce(player.transform.up * _weaponData.DropUpwardForce, ForceMode.Impulse);
            _rb.AddTorque(Util.GetRandomVector3(-1, 1) * _weaponData.DropTorqueForce);

            this.ToggleCollidersArrayAtDelay(_colliders, true, 0.5f);

        }

        // Reset parent when dropped
        public void ResetParent()
        {
            transform.SetParent(_originalParent);
        }

        internal void DisableWeapon()
        {
            _canShoot = false;
        }

        internal void EnableWeapon()
        {
            _canShoot = true;

        }

        public void OnAddBullets(int bulletCount)
        {
            _bulletsLeft += bulletCount;
            _bulletCountText.text = _bulletsLeft.ToString();
        }
    }

}
