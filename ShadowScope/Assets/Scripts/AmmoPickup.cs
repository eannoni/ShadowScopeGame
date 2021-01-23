using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    public int ammoPickupAmount = 10;

    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<PlayerController>().CollectedAmmoPickup(id, ammoPickupAmount);
    }
}
