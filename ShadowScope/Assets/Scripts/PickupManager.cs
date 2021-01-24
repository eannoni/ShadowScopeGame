using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance;

    public GameObject[] pickupObjects;
    Pickup[] pickups;

    [SerializeField] float respawnTime = 10f;

    void Awake()
    {
        Instance = this;
        pickups = new Pickup[pickupObjects.Length];

        // assign pick-up IDs
        for(int i = 0; i < pickupObjects.Length; ++i)
        {
            pickups[i] = pickupObjects[i].GetComponentInChildren<Pickup>();
            pickups[i].id = i;
        }
    }

    // iterates through all pickups, and checks to see if they should re-spawn
    void Update()
    {
        foreach (Pickup pickup in pickups)
        {
            if (!pickup.IsActive())
            {
                pickup.inactiveTime += Time.deltaTime;

                if (pickup.inactiveTime >= respawnTime)
                {
                    pickup.SetActive(true);
                }
            }
        }
    }

    public void DeactivatePickup(int id)
    {
        pickups[id].SetActive(false);
    }
}
