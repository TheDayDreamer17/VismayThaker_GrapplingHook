using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy _enemyPrefab;
        // Random time for spawning
        [SerializeField] private float _minEnemySpawnTime = 0.2f, _maxEnemySpawnTime = 2.5f;
        // enemy will be spawned randomly between -distance to +distance 
        [SerializeField] private float _enemySpawnDistanceMaxDistance = 25;
        private Vector3 _spawnPosition;
        private Coroutine _spawnEnemyCoroutine;
        private void Start()
        {
            _spawnEnemyCoroutine = StartCoroutine(SpawnEnemies());

        }
        private void OnEnable()
        {
            GameEvents.OnPlayerDie += OnPlayerDie;
            GameEvents.OnGameRestart += OnGameRestart;
        }
        private void OnDisable()
        {
            GameEvents.OnGameRestart -= OnGameRestart;
            GameEvents.OnPlayerDie -= OnPlayerDie;
        }

        private void OnGameRestart()
        {
            _spawnEnemyCoroutine = StartCoroutine(SpawnEnemies());
        }

        // Clear previous enemies and stop generation of new ones
        private void OnPlayerDie()
        {
            if (_spawnEnemyCoroutine != null)
                StopCoroutine(_spawnEnemyCoroutine);

            foreach (Transform item in transform)
            {
                item.GetComponent<Enemy>().KillEnemy();
            }
        }



        private void GetNewPosition()
        {
            _spawnPosition = Util.GetRandomVector3(-_enemySpawnDistanceMaxDistance, _enemySpawnDistanceMaxDistance);
            _spawnPosition.y = transform.position.y;

        }

        IEnumerator SpawnEnemies()
        {
            while (true)
            {
                GetNewPosition();
                Instantiate(_enemyPrefab, _spawnPosition, Quaternion.identity, transform);
                yield return new WaitForSeconds(Random.Range(_minEnemySpawnTime, _maxEnemySpawnTime));
            }
        }
    }
}
