using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private bool _stopSpawning = false;
    private bool _stopSpawningEnemies = false;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private int[] _powerUpFrequency = { 5,5,5,7,3,2,5};
    private int _enemiesToSpawn = 1;
    [SerializeField]
    private GameObject _boss;
    private int _bossWave = 6;
    private bool _bossSpawned = false;

    public void StartSpawning()
    {       
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(EnemyWaveRoutine());

    }

    public void SpawnBoss()
    {
        Instantiate(_boss, new Vector3(0,8f,0), Quaternion.identity); 
    }
    IEnumerator EnemyWaveRoutine()
    {
        while (_stopSpawning == false && _stopSpawningEnemies == false)
        {
            yield return new WaitUntil(() => _enemyContainer.transform.childCount == 0);

            if(_enemiesToSpawn >=_bossWave && _bossSpawned == false )
            {
                _stopSpawningEnemies = true;
                SpawnBoss();
                _bossSpawned = true;
                _enemiesToSpawn = 0; 
                yield break;
            }

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

    public void OnPlayerVictory()
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



}
