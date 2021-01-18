using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    Spawnpoint[] spawnpointsRedTeam;
    Spawnpoint[] spawnpointsBlueTeam;

    public GameObject redSpawnpoints;
    public GameObject blueSpawnpoints;

    void Awake()
    {
        Instance = this;
        // initialize arrays with child spawnpoints
        spawnpointsRedTeam = redSpawnpoints.GetComponentsInChildren<Spawnpoint>();
        spawnpointsBlueTeam = blueSpawnpoints.GetComponentsInChildren<Spawnpoint>();
    }

    public Transform GetSpawnpoint(int team)
    {
        // returns a random spawnpoint transform from spawnpoint array
        if(team == 0) // red team
        {
            return spawnpointsRedTeam[Random.Range(0, spawnpointsRedTeam.Length)].transform;
        }
        // blue team
        return spawnpointsBlueTeam[Random.Range(0, spawnpointsBlueTeam.Length)].transform;
    }
}
