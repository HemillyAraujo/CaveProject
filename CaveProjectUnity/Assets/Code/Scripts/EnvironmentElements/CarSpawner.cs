using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cave.EnvironmentElements
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool _isMoveForward = true;

        [SerializeField]
        private GameObject[] _carPrefabs;
        
        [SerializeField]
        private float _spawnAreaWidth;

        [Header("Spawn Settings")]
        [SerializeField]
        private Vector3 _spawnPostion;

        [SerializeField]
        private Vector3 _spawnRotation;

        [SerializeField]
        private float _despawnZ;

        [Header("Speed Settings")]
        [SerializeField]
        private float _speed;

        [Header("Spawn Interval Settings")]
        [SerializeField]
        private float _minSpawnInterval;

        [SerializeField]
        private float _maxSpawnInterval;

        [SerializeField]
        private int _maxCarsAtOnce;

        private List<GameObject> _activeCars;

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        private void Update()
        {
            Vector3 direction = _isMoveForward ? Vector3.forward : Vector3.back;

            for (int i = _activeCars.Count - 1; i >= 0; i--)
            {
                GameObject car = _activeCars[i];

                if (car == null)
                {
                    _activeCars.RemoveAt(i);
                    continue;
                }

                car.transform.Translate(direction * _speed * Time.deltaTime, Space.World);

                bool reachedDespawn = _isMoveForward
                    ? car.transform.position.z >= _despawnZ
                    : car.transform.position.z <= _despawnZ;

                if (reachedDespawn)
                {
                    DespawnCar(i);
                }
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float interval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
                yield return new WaitForSeconds(interval);

                bool underLimit = _maxCarsAtOnce <= 0 || _activeCars.Count < _maxCarsAtOnce;
                if (underLimit)
                {
                    SpawnCar();
                }
            }
        }

        private void SpawnCar()
        {
            if (_carPrefabs == null || _carPrefabs.Length == 0)
            {
                return;  
            } 

            GameObject randomPrefab = _carPrefabs[Random.Range(0, _carPrefabs.Length)];

            Vector3 spawnPos = new Vector3(_spawnPostion.x, _spawnPostion.y, _spawnPostion.z);

            Quaternion rotation = Quaternion.Euler(_spawnRotation);

            GameObject car = Instantiate(randomPrefab, spawnPos, rotation);
            _activeCars.Add(car);
        }

        private void DespawnCar(int index)
        {
            GameObject car = _activeCars[index];
            _activeCars.RemoveAt(index);
            Destroy(car);
        }
    }
}