using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGroup : MonoBehaviour
{

    public List<Transform> SpawnPoints;
    private List<bool> _spawnOccupied;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _spawnOccupied = new List<bool>();
        foreach (var p in SpawnPoints)
        {
            _spawnOccupied.Add(false);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _spawnOccupied.Count; i++)
        {
            _spawnOccupied[i] = false;
        }
    }

    public Vector3 GetPosition()
    {
        for (int i = 0; i < _spawnOccupied.Count; i++)
        {
            if (!_spawnOccupied[i])
            {
                _spawnOccupied[i] = true;
                return SpawnPoints[i].position;
            }
        }
        return Vector3.zero;
    }



}
