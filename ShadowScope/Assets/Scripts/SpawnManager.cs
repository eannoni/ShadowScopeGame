using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    Spawnpoint[] spawnpoints;

    void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>(); // initialize array with child spawnpoints
    }

    public Transform GetSpawnpoint()
    {
        // returns a random spawnpoint transform from spawnpoint array
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
