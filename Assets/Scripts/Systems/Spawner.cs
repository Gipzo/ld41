using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnemyType
{
    None = 0,
    Slow = 1,
    Frog = 2,
    Bird = 3,
    Robot = 4
}
[System.Serializable]
public enum SpawnerGroupType
{
    Top = 0,
    Right = 1,
    Left = 2,
    Bottom = 3
}

[System.Serializable]
public class SpawnWaveEnemy
{
    public SpawnerGroupType SpawnGroup;
    public EnemyType EnemyType;
    [Range(1, 5)]
    public int Count;
    public float Delay = 0;
}

[System.Serializable]
public class SpawnWave
{
    public List<SpawnWaveEnemy> Spawns;

    public EnemyType AdditionalEnemy;
    public float AdditionalRate = 0;
    public float AdditionalDelay = 1f;
    public BonusType FinalBonus;


    // public 

}

public class Spawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;
    public List<SpawnWave> Waves;
    public List<SpawnGroup> SpawnGroups;
    private List<Enemy> CurrentSpawnedEnemies;
    private List<Enemy> CurrentAdditionalEnemies;

    public bool SpawnEnabled = false;
    private SpawnWave _currentWave;
    private SpawnWaveEnemy _currentEnemy;
    public int _currentWaveIndex = 0;
    public int _currentWaveEnemyIndex = 0;
    private float _waveTimer = 0;
    private float _addTimeout = 0;

    public int CurrentWave
    {
        get
        {
            return _currentWaveIndex + 1;
        }
    }
    public int EnemiesLeft
    {
        get
        {
            return CurrentSpawnedEnemies.Count + CurrentAdditionalEnemies.Count;
        }
    }
    // Use this for initialization
    void Start()
    {

        CurrentSpawnedEnemies = new List<Enemy>();
        CurrentAdditionalEnemies = new List<Enemy>();
    }

    [ContextMenu("Start spawner")]
    public void StartSpawner()
    {
        SpawnEnabled = true;
        _waveTimer = 0;
        _currentWave = Waves[0];
        _currentWaveEnemyIndex = 0;
        _currentEnemy = _currentWave.Spawns[0];
        _addTimeout = _currentWave.AdditionalDelay;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        CurrentSpawnedEnemies.Remove(enemy);
        CurrentAdditionalEnemies.Remove(enemy);
    }

    bool CanSpawnAdditionalEnemy()
    {
        return CurrentSpawnedEnemies.Count > 0;
    }

    bool WaveCompleted()
    {
        if (CurrentSpawnedEnemies.Count > 0)
            return false;

        if (CurrentAdditionalEnemies.Count > 0)
            return false;

        if (_currentWaveEnemyIndex < _currentWave.Spawns.Count)
            return false;

        return true;
    }

    Enemy SpawnEnemy(EnemyType type, int spawn_group)
    {
        if (type == EnemyType.None)
            return null;
        var pos = SpawnGroups[spawn_group].GetPosition();
        int index = (int)type - 1;
        var go = GameObject.Instantiate(EnemyPrefabs[index], pos, Quaternion.identity);
        return go.GetComponent<Enemy>();

    }

    void SpawnAdditionalEnemy()
    {
        if (_addTimeout <= 0)
        {
            int group = Random.Range(0, SpawnGroups.Count);
            var e = SpawnEnemy(_currentWave.AdditionalEnemy, group);
            if (e != null)
                CurrentAdditionalEnemies.Add(e);

            SpawnGroups[group].Clear();
            _addTimeout = _currentWave.AdditionalRate;
        }
    }

    void SpawnWave()
    {
        if (_currentEnemy == null)
            return;
        _waveTimer = _currentEnemy.Delay;

        for (int i = 0; i < _currentEnemy.Count; i++)
        {
            var e = SpawnEnemy(_currentEnemy.EnemyType, (int)_currentEnemy.SpawnGroup);
            if (e != null)
                CurrentSpawnedEnemies.Add(e);
        }
        SpawnGroups[(int)_currentEnemy.SpawnGroup].Clear();

        _currentWaveEnemyIndex++;
        if (_currentWaveEnemyIndex < _currentWave.Spawns.Count)
        {
            _currentEnemy = _currentWave.Spawns[_currentWaveEnemyIndex];
        }
        else
        {
            _currentEnemy = null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_addTimeout >= 0)
        {
            _addTimeout -= Time.deltaTime;
        }
        if (_waveTimer >= 0)
        {
            _waveTimer -= Time.deltaTime;
        }

        if (!SpawnEnabled)
            return;

        if (WaveCompleted())
        {
            Bonus.SpawnBonus(_currentWave.FinalBonus, Vector3.zero);
            GameManager.Instance.WavePassed(_currentWaveIndex);
            _currentWaveIndex++;
            if (_currentWaveIndex >= Waves.Count)
            {
                GameManager.Instance.GameEnded(false);
                Debug.Log("Finished game");
                SpawnEnabled = false;
                return;
            }
            else
            {
                _currentWave = Waves[_currentWaveIndex];
                _addTimeout = _currentWave.AdditionalDelay;
                _currentEnemy = _currentWave.Spawns[0];
                _currentWaveEnemyIndex = 0;

            }
        }
        else
        {
            if (_waveTimer <= 0)
            {
                SpawnWave();
            }

            if (CanSpawnAdditionalEnemy())
            {
                SpawnAdditionalEnemy();
            }
        }

    }
}
