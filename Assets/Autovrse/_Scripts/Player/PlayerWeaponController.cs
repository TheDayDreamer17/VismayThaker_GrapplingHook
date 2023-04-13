using UnityEngine;
namespace Autovrse
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private Player _player;
        [SerializeField] private Weapon _currentWeapon = null;
        [SerializeField] private Transform _weaponHolderParent;
        private Transform _mainCamTransform;
        [SerializeField] private float xRotationOffset;
        public bool IsNoWeaponInHand => _currentWeapon == null;
        private void Awake()
        {
            _player = GetComponent<Player>();
        }
        private void Start()
        {
            _mainCamTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            GameEvents.OnCurrentWeaponDropped += DetachWeapon;
        }
        private void OnDisable()
        {
            GameEvents.OnCurrentWeaponDropped -= DetachWeapon;
        }

        private void LateUpdate()
        {
            if (!IsNoWeaponInHand)
            {
                _weaponHolderParent.transform.rotation = Quaternion.Euler(new Vector3(_mainCamTransform.eulerAngles.x + xRotationOffset, _weaponHolderParent.eulerAngles.y, _weaponHolderParent.eulerAngles.z));
            }
        }

        private void OnCollisionEnter(Collision other)
        {

            // Check if parent has IInventoryItem
            Weapon weapon = other.collider.GetComponentInParent<Weapon>();
            if (weapon != null && IsNoWeaponInHand)
            {
                AttachWeapon(weapon);
            }
        }
        public void AttachWeapon(Weapon weapon)
        {
            // make gun parent of player
            weapon.OnPick(_player);
            _currentWeapon = weapon;
            weapon.transform.SetParent(_weaponHolderParent);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }

        public void DetachWeapon()
        {
            if (IsNoWeaponInHand)
                return;
            // remove gun from player
            _currentWeapon.OnDrop(_player);
            _currentWeapon.ResetParent();
            _currentWeapon = null;
        }

        public void DisableCurrentWeapon()
        {
            _currentWeapon?.DisableWeapon();
        }
        public void EnableCurrentWeapon()
        {
            _currentWeapon?.EnableWeapon();
        }
        public void ReloadWeapon()
        {

            _currentWeapon?.ReloadWeapon();
        }
    }
}
