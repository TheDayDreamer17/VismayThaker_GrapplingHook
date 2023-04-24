using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class DropLocationSelector : Singleton<DropLocationSelector>
    {
        [SerializeField] private List<GameObject> _locations;

        [SerializeField] private DropHereIndicator _dropHereIndicatorPrefab;
        [SerializeField] private GameObject _dropLocation;
        private DropHereIndicator _instantiatedDropHereIndicator;
        [SerializeField] private Player _player;

        [SerializeField] private float _minDistanceFromPlayer = 30;
        private float _maxheight = 222;

        private void Start()
        {
            // FindMaxHeight();
            GenerateDropLocation();

        }

        private void GenerateDropLocation()
        {
            if (_dropLocation == null)
                return;
            _instantiatedDropHereIndicator = Instantiate(_dropHereIndicatorPrefab, _dropLocation.transform.position, Quaternion.identity, transform);
            _instantiatedDropHereIndicator.SetupIndicator(_dropLocation, _maxheight);
        }


        // Calculated once to get idea of what is the max height for a case where a destination building is behind the max height building, And we can still see drop here mark
        private void FindMaxHeight()
        {
            Collider collider;
            _maxheight = 0;
            foreach (var item in _locations)
            {
                collider = item.GetComponent<Collider>();
                if (collider != null)
                {
                    _maxheight = collider.bounds.max.y > _maxheight ? collider.bounds.max.y : _maxheight;
                }
                collider = null;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnGameNextLevel += SelectNextLocaiton;
            GameEvents.OnGameRestart += GenerateDropLocation;
            GameEvents.OnPlayerSuccessful += ResetDropPoint;
            GameEvents.OnPlayerUnSuccessful += ResetDropPoint;
        }
        private void OnDisable()
        {
            GameEvents.OnGameNextLevel -= SelectNextLocaiton;
            GameEvents.OnGameRestart -= GenerateDropLocation;
            GameEvents.OnPlayerSuccessful -= ResetDropPoint;
            GameEvents.OnPlayerUnSuccessful -= ResetDropPoint;
        }

        private void ResetDropPoint()
        {
            Destroy(_instantiatedDropHereIndicator.gameObject);
        }

        // When Game is restarted a new drop location is selected
        public void SelectNextLocaiton()
        {
            _dropLocation = null;
            UpdateNextLocation();
        }

        // Update drop location on new level accessed
        public void UpdateNextLocation()
        {
            do
            {
                _dropLocation = GetRandomLocation();
            } while (Vector3.Distance(_dropLocation.transform.position, _player.transform.position) < _minDistanceFromPlayer);
        }

        private GameObject GetRandomLocation()
        {
            return _locations[Random.Range(0, _locations.Count)];
        }

    }
}
