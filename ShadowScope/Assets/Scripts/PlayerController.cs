﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    PhotonView pv;
    public Transform firePoint;
    public Transform laserFirePoint;
    public LineRenderer laserLinePrefab;

    [Header("Team Info")]
    public int team; // 0: red, 1: blue
    public Sprite redTeam;
    public Sprite blueTeam;

    float horizontal, vertical;
    float moveLimiter = 0.7f; // limit diagonal speed
    float rotationSpeed = 100f;


    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    public bool sprinting = false;
    const float maxHealth = 100f;
    float currHealth = maxHealth;

    PlayerManager playerManager;

    Vector2 mousePos;

    //Laserstuff:
    public float fireRate = 0.25f;
    public float weaponRange = 10000f;
    private float shotDuration = 0.07f; // how long the bullet trail is enabled
    private float nextFire; // amount of time before next fire is allowed

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        // gets player manager
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        SetSprite();
    }

    void Start()
    {
        if (!pv.IsMine) // destroy cameras for all other players?
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(body); // this prevents glitchy movement by destroying RigidBody calculations for all other players.
        }
        walkSpeed = 7.0f;
        sprintSpeed = 15.0f;
    }

    void Update()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        sprinting = Input.GetKey(KeyCode.LeftShift);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) // Left click fires
        {
            nextFire = Time.time + fireRate; //updating time when player can shoot next

            Shoot();
        }
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;
        Move();
        Rotate();
    }

    void Rotate()
    {
        Vector2 lookDirection = mousePos;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


    // Movement mechanics can be reworked, this is just for temporary testing purposes
    void Move()
    {
        if (horizontal != 0 && vertical != 0) // if moving diagonally
        {
            // limit diagonal movement to move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (sprinting)
            body.velocity = new Vector2(horizontal * sprintSpeed, vertical * sprintSpeed);
        else
            body.velocity = new Vector2(horizontal * walkSpeed, vertical * walkSpeed);
    }

    void Shoot()
    {
        Vector3 startPoint; // holds start point info that will be sent to all other clients
        Vector3 endPoint; // holds end point info that will be sent to all other clients

        // create raycast
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, mousePos);

        // assign startPoint
        startPoint = laserFirePoint.position;

        if (hit) // if hit something
        {
            // if hit a player
            if(hit.collider.tag == "Player")
            {
                // deal damage
                hit.collider.gameObject.GetComponent<PlayerController>().TakeDamage(10.0f);
            }

            // assign endPoint
            endPoint = hit.point;
        }
        else // if didn't hit anything
        {
            // assign endPoint to weapon's range
            endPoint = (firePoint.position + (laserFirePoint.transform.right * weaponRange));
            endPoint.z = 0;
        }

        // animate line
        ShootLine(startPoint, endPoint);
    }


    // tells all clients to draw a line from startPos to endPos
    public void ShootLine(Vector3 startPos, Vector3 endPos)
    {
        pv.RPC("RPC_ShootLine", RpcTarget.All, startPos, endPos);
    }

    [PunRPC]
    void RPC_ShootLine(Vector3 startPos, Vector3 endPos)
    {
        ShotEffect(startPos, endPos);
    }

    // draws line from startPos to endPos for a given amount of time
    private void ShotEffect(Vector3 startPos, Vector3 endPos)
    {
        // instantiate and initialize LineRenderer prefab
        LineRenderer laserGO = Instantiate(laserLinePrefab);
        laserGO.SetPosition(0, startPos);
        laserGO.SetPosition(1, endPos);

        // destroy instantiated LineRenderer
        Destroy(laserGO.gameObject, shotDuration);
    }



    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {

        if (!pv.IsMine)
            return;

        currHealth -= damage;
        Debug.Log("Took damage.");
        Debug.Log("Current HP: " + currHealth);

        if(currHealth <= 0)
            Die();
    }

    void SetSprite()
    {
        if(team == 0)
        {
            Debug.Log("Team color set to: red");
            GetComponent<SpriteRenderer>().sprite = redTeam;
        }
        else if (team == 1)
        {
            Debug.Log("Team color set to: blue");
            GetComponent<SpriteRenderer>().sprite = blueTeam;
        }
        else
        {
            Debug.LogError("ERROR: unknown team number, cannot assign player sprite");
        }
    }

    void Die()
    {
        Debug.Log("You died");
        playerManager.Die();
    }
}
