using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private int[] _powerUpFrequency = { 5,5,5,7,3,2,5};
    private int _enemiesToSpawn = 1;



    public void StartSpawning()
    {
       // StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(EnemyWaveRoutine());
    }

    IEnumerator EnemyWaveRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitUntil(() => _enemyContainer.transform.childCount == 0);
            for (int i = 0; i < _enemiesToSpawn; i++)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }
            _enemiesToSpawn++;
            yield return new WaitForSeconds(2f);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    //Balanced spawning 
    int GetRandomPowerUpIndex()
    {
        int totalFrequency = 0;
        foreach(var frequency in _powerUpFrequency)
        {
            totalFrequency += frequency;
        }
        int randomValue = Random.Range(0, totalFrequency);
        int currentFrequency = 0;
        for(int i=0; i<_powerUpFrequency.Length; i++)
        {
            currentFrequency += _powerUpFrequency[i];
            if (randomValue < currentFrequency)
            {
                return i;
            }
        }
        return 0;
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            int index  = GetRandomPowerUpIndex();
            Debug.Log(index);
            Instantiate(_powerups[index], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }


    IEnumerator SpawnRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(3.0f);
        }

    }
}
