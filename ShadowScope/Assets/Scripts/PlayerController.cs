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
    float rotationSpeed = 100f;

    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    public bool sprinting = false;
    public Camera mainCamera;
    Vector2 mousePos;

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
        }
        walkSpeed = 7.0f;
        sprintSpeed = 15.0f;
        //mainCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        sprinting = Input.GetKey(KeyCode.LeftShift);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1")) // Left click fires
        {
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
        Vector2 lookDirection = mousePos;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);  
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, transform.rotation.ToEulerAngles(), out hit)) // position, direction, output to variable hit, and range
        {
            // Section does not work as far as I've tested.
            Debug.Log("Hit: " + hit.transform.name); // Prints the name of the object it hits if successful.
            Debug.Log("Hit something"); // Tests if a collision was detected even if the object has no name.
        }

        Debug.Log("Origin of raycast:" + this.transform.position); // Prints raycast origin into console and confirms the raycast was sucessful
        Debug.Log("Direction:" + transform.rotation.ToEulerAngles()); // Prints the direction of the raycast.

    }
}
