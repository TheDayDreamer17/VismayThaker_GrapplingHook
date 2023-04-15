using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Autovrse
{
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField] private Platform _platformPrefab;
        [SerializeField] private GrapableObject _grapablePrefab;
        [SerializeField] private float _grapableObjectHeightOffset = 10f;
        // this is minimum distance from previous Platform where new platform will be spawned 
        [SerializeField] private float _platformSpawnMinDistance = 10;
        // this is maximum distance from previous Platform where new platform will be spawned
        [SerializeField] private float _platformSpawnMaxDistance = 16;

        [SerializeField] private float _platformMinSeperationDistance = 25;

        // this is minimum and maximum amount of platforms spawned at a time
        [SerializeField] private Vector2Int _platformsSpawnCount = new Vector2Int(3, 6);
        private Vector3 _spawnPosition;
        private Coroutine _spawnPlatformCoroutine;
        [SerializeField] private Platform _startingPlatform;
        private List<Platform> _platformPool = new List<Platform>();
        private List<GrapableObject> _grapableObjectPool = new List<GrapableObject>();
        [SerializeField] private int _poolAmount = 15;

        // Action called by final platform when player reaches there
        public static System.Action OnFinalPlatformReached;

        private Vector3 _lastGeneratedPlatformPosition;
        private List<Platform> _lastGeneratedPlatformList = new List<Platform>();
        private List<GrapableObject> _lastGeneratedGrapableList = new List<GrapableObject>();
        private Platform _lastGeneratedFinalPlatform;
        private void Start()
        {
            GenerateNewPlatforms();
        }

        private void GenerateNewPlatforms()
        {
            _spawnPlatformCoroutine = StartCoroutine(SpawnPlatforms());
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerDie += OnPlayerDie;
            GameEvents.OnGameRestart += OnGameRestart;
            OnFinalPlatformReached += GenerateNewPlatforms;
        }
        private void OnDisable()
        {
            GameEvents.OnGameRestart -= OnGameRestart;
            GameEvents.OnPlayerDie -= OnPlayerDie;
            OnFinalPlatformReached -= GenerateNewPlatforms;
        }

        private void OnGameRestart()
        {
            _lastGeneratedPlatformPosition = _startingPlatform.transform.position;
            _lastGeneratedGrapableList.Clear();
            _lastGeneratedPlatformList.Clear();
            _lastGeneratedFinalPlatform = null;
            _spawnPlatformCoroutine = StartCoroutine(SpawnPlatforms());
            // if fell then go to starting platform
            _startingPlatform.Show();
            _startingPlatform.PutPlayerOnThisPlatform();

        }

        // Clear previous Platforms and stop generation of new ones
        private void OnPlayerDie()
        {
            _startingPlatform.Hide();
            if (_spawnPlatformCoroutine != null)
                StopCoroutine(_spawnPlatformCoroutine);

            foreach (Platform item in _platformPool)
            {
                item.Hide();
            }
            foreach (GrapableObject item in _grapableObjectPool)
            {
                item.Hide();
            }
        }

        #region Platform and Grapable object Pool Methods 
        private Platform GetFromPlatformPool()
        {
            Platform platform = _platformPool.Find(platform => platform.IsHidden);
            return platform;
        }

        private void AddToPlatformPool(Platform platform)
        {
            _platformPool.Add(platform);
        }

        private GrapableObject GetFromGrapablePool()
        {
            GrapableObject grapableObject = _grapableObjectPool.Find(grapableObject => grapableObject.IsHidden);
            return grapableObject;
        }

        private void AddToGrapablePool(GrapableObject grapableObject)
        {
            _grapableObjectPool.Add(grapableObject);
        }
        #endregion

        // Get New platform position
        private void GetNewPosition()
        {
            if (_platformPool.Count == 0)
                _spawnPosition = _startingPlatform.transform.position + Util.GetRandomPositionInTorus(_platformSpawnMinDistance, _platformSpawnMaxDistance);
            else
                _spawnPosition = _lastGeneratedPlatformPosition + Util.GetRandomPositionInTorus(_platformSpawnMinDistance, _platformSpawnMaxDistance);

            _spawnPosition.y = transform.position.y;
            if (_platformPool.Exists(platform => !platform.IsHidden && Vector3.Distance(platform.transform.position, _spawnPosition) < _platformMinSeperationDistance) ||
            (!_startingPlatform.IsHidden && Vector3.Distance(_startingPlatform.transform.position, _spawnPosition) < _platformMinSeperationDistance))
                GetNewPosition();
        }

        IEnumerator SpawnPlatforms()
        {
            HidePreviousPlatformsAndGrapablePoints();
            int platformCount = Random.Range(_platformsSpawnCount.x, _platformsSpawnCount.y);
            if (_lastGeneratedPlatformList.Count > 0)
                _lastGeneratedFinalPlatform = _lastGeneratedPlatformList.Last();

            // Clear last generated platform and graple object list
            _lastGeneratedPlatformList.Clear();
            _lastGeneratedGrapableList.Clear();
            // Add final platfrom from previous generation if any to remove it in this iteration
            if (_lastGeneratedFinalPlatform != null)
                _lastGeneratedPlatformList.Add(_lastGeneratedFinalPlatform);

            for (int i = 0; i < platformCount; i++)
            {
                GetNewPosition();

                _lastGeneratedPlatformPosition = _spawnPosition;
                Platform platform;
                if (_platformPool.Count >= _poolAmount)
                {
                    platform = GetFromPlatformPool();
                    platform.Show();
                    platform.transform.position = _spawnPosition + Vector3.down * 10;
                }
                else
                {
                    platform = Instantiate(_platformPrefab, _spawnPosition + Vector3.down * 10, _platformPrefab.transform.rotation, transform);
                    AddToPlatformPool(platform);
                }

                // Generate grapable point only between two platform

                if (i == 0)
                    GenerateGrapableObject(_lastGeneratedFinalPlatform != null ? _lastGeneratedFinalPlatform : _startingPlatform, platform);
                else
                    GenerateGrapableObject(_lastGeneratedPlatformList[_lastGeneratedPlatformList.Count - 1], platform);

                _lastGeneratedPlatformList.Add(platform);

                platform.Setup(_spawnPosition, i == platformCount - 1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void GenerateGrapableObject(Platform first, Platform second)
        {
            // Find mid point of both platforms to put Grapable object
            Vector3 point = Vector3.Lerp(first.transform.position, second.transform.position, 0.5f);
            point.y += _grapableObjectHeightOffset;
            GrapableObject grapableObject;
            if (_grapableObjectPool.Count > _poolAmount)
            {
                grapableObject = GetFromGrapablePool();
                grapableObject.Show();
                grapableObject.transform.position = point;
            }
            else
            {
                grapableObject = Instantiate(_grapablePrefab, point, Quaternion.identity, transform);
                AddToGrapablePool(grapableObject);
            }

            _lastGeneratedGrapableList.Add(grapableObject);

        }

        private void HidePreviousPlatformsAndGrapablePoints()
        {
            if (_platformPool.Count > 0)
                _startingPlatform.Hide();
            for (int i = 0; i < _lastGeneratedPlatformList.Count - 1; i++)
            {
                _lastGeneratedPlatformList[i].Hide();
            }
            foreach (GrapableObject item in _lastGeneratedGrapableList)
            {
                item.Hide();
            }
        }

    }
}
