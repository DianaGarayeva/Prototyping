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

    private int _enemiesToSpawn = 1;



    public void StartSpawning()
    {
       // StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(EnemyWaveRoutine());
    }

    public void EnemyWave()
    {
        for (int i = 0; i < _enemiesToSpawn; i++)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
        }
        _enemiesToSpawn++;
    }

    IEnumerator EnemyWaveRoutine()
    {
        while (_stopSpawning == false)
        {
            yield return new WaitUntil(() => _enemyContainer.transform.childCount == 0);
            EnemyWave();
            yield return new WaitForSeconds(2f);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }



    

    IEnumerator SpawnRoutine()
    {
        //yield return new WaitForSeconds(3.0f);
        while (_stopSpawning==false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(3.0f);
        }
        
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            float chance = Random.Range(0f, 1f);
            if (chance <= 0.2)
            {
                Debug.Log(chance);
                Instantiate(_powerups[5], posToSpawn, Quaternion.identity);
            }

            else
            {
                int randomPowerUp;
                do
                {
                    randomPowerUp = Random.Range(0, _powerups.Length);
                } while (randomPowerUp == 5);
                Debug.Log(chance);
                Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }
}
