using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    PhotonView pv;

    float horizontal, vertical;
    float moveLimiter = 0.7f; // limit diagonal speed

    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    public bool sprinting = false;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!pv.IsMine) // destroy cameras for all other players?
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(body); // this prevents glitchy movement by destroying RigidBody calculations for all other players.
        } else
        {

            GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
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
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;
        Move();
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
}
